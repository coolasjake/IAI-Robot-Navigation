using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class BOT_BreadthFirst : Robot
    {
        protected List<Node> BFSNodes = new List<Node>();

        public BOT_BreadthFirst(Environment Env) : base(Env) { name = "Breadth First"; }

        protected override void StartUpdateSolution()
        {
            BaseStartUS();

            //Console.WriteLine("Setting Variables");
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

            Console.WriteLine("Cancelling Breadth First");
        }

        protected override void StepUpdateSolution()
        {
            Step();
        }

        protected override void Step()
        {
            ++loops;
            if (nodeIndex >= BFSNodes.Count)
            {
                end = true;
                Console.WriteLine("Search ended without finding solution. I = " + nodeIndex + ", C = " + BFSNodes.Count);
            }
            else
            {
                Node currentNode = BFSNodes[nodeIndex];
                virtualState = currentNode.state;
                //Console.WriteLine("Checking Position: " + currentNode);
                //transform.position = env.CellRepPos(currentNode.state);
                cost = currentNode.cost + 1;
                CellState cell = env.GetCellState(currentNode.state);
                //Console.WriteLine("Node is: " + cell);
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
                        if (BFSNodes.Find(X => X.state == newNode.state) == null)
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
        }

        protected override int LogMemory()
        {
            return BFSNodes.Count;
            Console.WriteLine("End Memory Used: " + BFSNodes.Count + " Nodes in tree");
        }
    }

}
