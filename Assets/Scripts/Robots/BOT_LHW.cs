using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOT_LHW : Robot
{
    protected List<Node> usedNodes = new List<Node>();

    public bool followLeft = true;

    //public BOT_LHW(Environment Env) : base(Env) { name = "Wall Follower"; }

    protected override void StartUpdateSolution()
    {
        BaseStartUS();

        //BuildDebug.Log("Setting Variables");
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

        BuildDebug.Log("Cancelling Depth First");
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    protected void Step()
    {
        ++loops;
        if (nodeIndex >= path.Count || nodeIndex < 0)
        {
            end = true;
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
                //BuildDebug.Log("Found goal: " + currentNode.state);
                foundGoal = true;
                return;
            }
            else if (cell == CellState.Free || cell == CellState.Robot)
            {
                //Try the node to the left (relative to last move)
                Vector2Int delta = ClockwiseDelta(currentNode.actionUsed, followLeft);
                for (int i = 0; i < 3; ++i)
                {
                    Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                    if (usedNodes.Find(X => X.state == currentNode.state + delta) == null)
                    {
                        usedNodes.Add(newNode);
                        path.Add(newNode);
                        endOfBranch = false;
                        break;
                    }
                    delta = ClockwiseDelta(delta, !followLeft);
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

    private Vector2Int ClockwiseDelta(Vector2Int original)
    {
        if (original.x < 0)      //Left -> Up
            return new Vector2Int(0, -1);
        else if (original.y < 0) //Up -> Right
            return new Vector2Int(1, 0);
        else if (original.x > 0) //Right -> Down
            return new Vector2Int(0, 1);
        else if (original.y > 0) //Down -> Left
            return new Vector2Int(-1, 0);
        else //Default
            return new Vector2Int(0, -1);//Up
    }

    private Vector2Int ClockwiseDelta(Vector2Int original, bool reversed)
    {
        if (reversed)   //ANTI-CLOCKWISE
        {
            if (original.x < 0)      //L
                return new Vector2Int(0, 1);//Down
            else if (original.y < 0) //U
                return new Vector2Int(-1, 0);//Left
            else if (original.x > 0) //R
                return new Vector2Int(0, -1);//Up
            else if (original.y > 0) //D
                return new Vector2Int(1, 0);//Right
            else //Default
                return new Vector2Int(0, -1);//Up
        }

        return ClockwiseDelta(original);
    }

    protected override void CreateUpdatePath()
    {
        //For DFS the path is the same as the 'current branch'.
        
        GenerateLineVisual();

        BaseEndUS();
    }

    protected override void LogMemory()
    {
        BuildDebug.Log("End Memory Used: " + usedNodes.Count + " Nodes in complete tree + "
            + path.Count + " Nodes in main Branch = " + (path.Count + usedNodes.Count));
    }
}