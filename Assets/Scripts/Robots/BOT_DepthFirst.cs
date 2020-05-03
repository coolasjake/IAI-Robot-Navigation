using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOT_DepthFirst : Robot
{
    protected List<Node> usedNodes = new List<Node>();

    public override void FindSolution()
    {
        Debug.Log("Beginning Search");
        int loops = 0;
        int level = 0;
        //int nodeIndex = 0;
        bool end = false;
        bool foundGoal = false;
        usedNodes.Add(new Node(initState, Vector2Int.zero, level, -1));
        while (!end && loops < MaxLoops)
        {
            Step();
        }

        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = usedNodes[nodeIndex];
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, usedNodes[nextNode.parentIndex]);
                nextNode = usedNodes[nextNode.parentIndex];
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
        path.Clear();
        path.Add(new Node(initState, Vector2Int.zero, cost, -1));
        usedNodes.Clear();
        usedNodes.Add(path[0]);
    }

    protected override void CancelUpdateSolution()
    {
        BaseEndUS();

        Debug.Log("Cancelling Depth First");
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    private void Step()
    {
        ++loops;
        if (nodeIndex >= path.Count || nodeIndex < 0)
        {
            end = true;
            Debug.LogWarning("Search ended without finding solution");
        }
        else
        {
            Node currentNode = path[nodeIndex];
            virtualState = currentNode.state;
            transform.position = env.CellRepPos(currentNode.state);
            cost = currentNode.level + 1;
            CellState cell = env.GetCellState(currentNode.state);
            bool endOfBranch = true;
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
                    if (usedNodes.Find(X => X.state == currentNode.state + delta) == null)
                    {
                        usedNodes.Add(newNode);
                        path.Add(newNode);
                        endOfBranch = false;
                        break;
                    }
                }
            }

            if (endOfBranch)
            {
                path.Remove(currentNode);
                --nodeIndex;
            }
            else
                ++nodeIndex;
        }
    }

    protected override void CreateUpdatePath()
    {
        //For DFS the path is the same as the 'current branch'.

        GenerateLineVisual();

        BaseEndUS();
    }

    protected override void LogMemory()
    {
        BuildDebug.Log("End Memory Used: " + usedNodes.Count +" Nodes in complete tree + "
            + path.Count + " Nodes in main Branch = " + (path.Count + usedNodes.Count));
    }
}

