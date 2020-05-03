using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOT_AStar : Robot
{
    public List<Node> nodeQueue = new List<Node>();
    public List<Node> triedNodes = new List<Node>();

    public override void FindSolution()
    {
        Debug.Log("Beginning Search");
        int loops = 0;
        int level = 0;
        //int nodeIndex = 0;
        bool end = false;
        bool foundGoal = false;
        nodeQueue.Add(new Node(initState, Vector2Int.zero, level, -1));
        while (!end && loops < MaxLoops)
        {
            Step();
        }

        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = nodeQueue[nodeIndex];
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, nodeQueue[nextNode.parentIndex]);
                nextNode = nodeQueue[nextNode.parentIndex];
            }
        }
    }

    protected override void StartUpdateSolution()
    {
        BaseStartUS();

        Debug.Log("Setting Variables");
        cost = 0;
        //nodeIndex = 0;
        end = false;
        foundGoal = false;
        nodeQueue.Clear();
        triedNodes.Clear();
        Node startNode = new Node(initState, Vector2Int.zero, cost, -1);
        nodeQueue.Add(startNode);
    }

    protected override void CancelUpdateSolution()
    {
        BaseEndUS();

        Debug.Log("Cancelling Breadth First");
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    private void Step()
    {
        ++loops;
        if (nodeQueue.Count == 0)
        {
            end = true;
            Debug.LogWarning("Search ended without finding solution. I = " + nodeIndex + ", C = " + nodeQueue.Count);
        }
        else
        {
            Node currentNode = nodeQueue[0];
            //Remove the first node from the list (this node).
            triedNodes.Add(currentNode);
            nodeIndex = triedNodes.Count - 1;
            nodeQueue.RemoveAt(0);
            virtualState = currentNode.state;
            transform.position = env.CellRepPos(currentNode.state);
            //cost = FindCost(currentNode.state);
            CellState cell = env.GetCellState(currentNode.state);
            if (cell == CellState.Goal)
            {
                firstGoal = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                end = true;
                Debug.Log("Found goal: " + currentNode.state);
                foundGoal = true;
                return;
            }
            else if (cell == CellState.Free || cell == CellState.Robot)
            {
                foreach (Vector2Int delta in deltas)
                {
                    cost = FindCost(currentNode.state + delta);
                    Node newNode = new Node(currentNode.state + delta, delta, currentNode.level + 1, cost, nodeIndex);
                    Node existing = nodeQueue.Find(X => X.state == newNode.state);
                    if (existing != null)
                    {
                        if (Heuristic(newNode) < Heuristic(existing))
                            nodeQueue.Remove(existing);
                        else
                            continue;
                    }

                    if (triedNodes.Find(X => X.state == newNode.state) == null)
                    {
                        int index = nodeQueue.Count - 1;
                        while (index > 0 && Heuristic(newNode) < Heuristic(nodeQueue[index]))
                            --index;
                        nodeQueue.Insert(index + 1, newNode);
                    }
                }
            }
        }
    }

    private int Heuristic(Node n)
    {
        return n.cost + n.level;
    }

    private int FindCost(Vector2Int state)
    {
        int c = 1000000;
        foreach (Vector2Int goal in env.goals)
        {
            int costToGoal = ObliqueDist(state, goal);
            if (costToGoal < c)
                c = costToGoal;
        }
        return c;
    }

    protected override void CreateUpdatePath()
    {
        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = firstGoal;
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, triedNodes[nextNode.parentIndex]);
                nextNode = triedNodes[nextNode.parentIndex];
            }
        }

        GenerateLineVisual();

        BaseEndUS();
    }

    protected override void LogMemory()
    {
        BuildDebug.Log("End Memory Used: " + nodeQueue.Count + " Nodes in queue, " + triedNodes.Count + " Nodes in list = " + (nodeQueue.Count + triedNodes.Count));
    }
}
