using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOT_NoCurationBFS : Robot
{
    protected List<Node> BFSNodes = new List<Node>();

    public override void FindSolution()
    {
        Debug.Log("Beginning Search");
        int loops = 0;
        int level = 0;
        //int nodeIndex = 0;
        bool end = false;
        bool foundGoal = false;
        BFSNodes.Add(new Node(initState, Vector2Int.zero, level, -1));
        while (!end && loops < MaxLoops)
        {
            Step();
        }

        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = BFSNodes[nodeIndex];
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, BFSNodes[nextNode.parentIndex]);
                nextNode = BFSNodes[nextNode.parentIndex];
            }
        }
    }

    protected override void StartUpdateSolution()
    {
        BaseStartUS();

        Debug.Log("Setting Variables");
        cost = 0;
        nodeIndex = 0;
        end = false;
        foundGoal = false;
        BFSNodes.Clear();
        BFSNodes.Add(new Node(initState, Vector2Int.zero, cost, -1));
    }

    protected override void CancelUpdateSolution()
    {
        BaseEndUS();

        Debug.Log("Cancelling Breadth First No Curation");
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    private void Step()
    {
        ++loops;
        if (nodeIndex >= BFSNodes.Count)
        {
            end = true;
            Debug.LogWarning("Search ended without finding solution");
        }
        else
        {
            Node currentNode = BFSNodes[nodeIndex];
            virtualState = currentNode.state;
            transform.position = env.CellRepPos(currentNode.state);
            cost = currentNode.cost + 1;
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
                    Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                    BFSNodes.Add(newNode);
                }
            }
            ++nodeIndex;
        }
    }

    protected override void CreateUpdatePath()
    {
        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = firstGoal;
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, BFSNodes[nextNode.parentIndex]);
                nextNode = BFSNodes[nextNode.parentIndex];
            }
        }

        GenerateLineVisual();

        BaseEndUS();
    }

    protected override void LogMemory()
    {
        BuildDebug.Log("End Memory Used: " + BFSNodes.Count + " Nodes in tree");
    }
}
