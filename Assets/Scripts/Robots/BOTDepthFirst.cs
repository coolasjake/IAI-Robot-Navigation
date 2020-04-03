using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTDepthFirst : Robot
{
    public List<Node> usedNodes = new List<Node>();
    public List<Node> branch = new List<Node>();
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
        usedNodes.Add(new Node(state, Vector2Int.zero, level, -1));
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
        level = 0;
        nodeIndex = 0;
        end = false;
        foundGoal = false;
        branch.Clear();
        branch.Add(new Node(state, Vector2Int.zero, level, -1));
        usedNodes.Clear();
        usedNodes.Add(branch[0]);
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    private void Step()
    {
        ++loops;
        if (nodeIndex >= branch.Count || nodeIndex < 0)
        {
            end = true;
            Debug.LogWarning("Search ended without finding solution");
        }
        else
        {
            Node currentNode = branch[nodeIndex];
            Rep.anchoredPosition = env.CellRepPos(currentNode.state);
            level = currentNode.level + 1;
            CellState cell = env.GetCellState(currentNode.state);
            bool endOfBranch = true;
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
                    if (usedNodes.Find(X => X.state == currentNode.state + delta).exists == false)
                    {
                        usedNodes.Add(newNode);
                        branch.Add(newNode);
                        endOfBranch = false;
                        break;
                    }
                }
            }

            if (endOfBranch)
            {
                branch.Remove(currentNode);
                --nodeIndex;
            }
            else
                ++nodeIndex;
        }
    }

    protected override void CreateUpdatePath()
    {
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

        BaseEndUS();
    }
}

