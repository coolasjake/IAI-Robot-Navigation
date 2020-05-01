using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class BOT_DepthFirst : Robot
    {
        protected List<Node> usedNodes = new List<Node>();

        public BOT_DepthFirst(Environment Env) : base(Env) { name = "Depth First"; }

        protected override void StartUpdateSolution()
        {
            BaseStartUS();

            //Console.WriteLine("Setting Variables");
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

            Console.WriteLine("Cancelling Depth First");
        }

        protected override void StepUpdateSolution()
        {
            Step();
        }

        protected override void Step()
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
                //transform.position = env.CellRepPos(currentNode.state);
                cost = currentNode.cost + 1;
                CellState cell = env.GetCellState(currentNode.state);
                bool endOfBranch = true;
                if (cell == CellState.Goal)
                {
                    firstGoal = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                    end = true;
                    //Console.WriteLine("Found goal: " + currentNode.state);
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
        }

        protected override int LogMemory()
        {
            return usedNodes.Count;
            Console.WriteLine("End Memory Used: " + usedNodes.Count + " Nodes in complete tree + "
                + path.Count + " Nodes in main Branch = " + (path.Count + usedNodes.Count));
        }
    }


}
