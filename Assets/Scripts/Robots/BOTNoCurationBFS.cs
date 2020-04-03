using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTNoCurationBFS : Robot
{
    public List<Node> BFSNodes = new List<Node>();
    public List<Node> path = new List<Node>();

    public int level = 0;
    public int nodeIndex = 0;
    private bool foundGoal = false;

    public override void FindSolution()
    {
        Debug.Log("Beginning Search");
        int loops = 0;
        int level = 0;
        //int nodeIndex = 0;
        bool end = false;
        bool foundGoal = false;
        BFSNodes.Add(new Node(state, Vector2Int.zero, level, -1));
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
        level = 0;
        nodeIndex = 0;
        end = false;
        foundGoal = false;
        BFSNodes.Clear();
        BFSNodes.Add(new Node(state, Vector2Int.zero, level, -1));
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
            Rep.anchoredPosition = env.CellRepPos(currentNode.state);
            level = currentNode.level + 1;
            CellState cell = env.GetCellState(currentNode.state);
            if (cell == CellState.Goal)
            {
                end = true;
                Debug.Log("Found goal: " + currentNode.state);
                foundGoal = true;
                return;
            }
            else if (cell == CellState.Free || cell == CellState.Robot)
            {
                foreach (Vector2Int delta in deltas)
                {
                    Node newNode = new Node(currentNode.state + delta, delta, level, nodeIndex);
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
            Node nextNode = BFSNodes[nodeIndex];
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, BFSNodes[nextNode.parentIndex]);
                nextNode = BFSNodes[nextNode.parentIndex];
            }
        }

        BaseEndUS();
    }
}
