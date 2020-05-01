using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class BOT_GBFS : Robot
    {
        protected List<Node> nodeQueue = new List<Node>();
        protected List<Node> triedNodes = new List<Node>();

        public BOT_GBFS(Environment Env) : base(Env) { name = "GBFS"; }

        protected override void StartUpdateSolution()
        {
            BaseStartUS();

            //Console.WriteLine("Setting Variables");
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

            Console.WriteLine("Cancelling Breadth First");
        }

        protected override void StepUpdateSolution()
        {
            Step();
        }

        protected override void Step()
        {
            ++loops;
            if (nodeQueue.Count == 0)
            {
                end = true;
            }
            else
            {
                Node currentNode = nodeQueue[0];
                //Remove the first node from the list (this node).
                triedNodes.Add(currentNode);
                nodeIndex = triedNodes.Count - 1;
                nodeQueue.RemoveAt(0);
                virtualState = currentNode.state;
                //transform.position = env.CellRepPos(currentNode.state);
                //cost = FindCost(currentNode.state);
                CellState cell = env.GetCellState(currentNode.state);
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
                        cost = FindCost(currentNode.state + delta);
                        Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                        if (triedNodes.Find(X => X.state == newNode.state) == null && nodeQueue.Find(X => X.state == newNode.state) == null)
                        {
                            int index = nodeQueue.Count;
                            Node nodeBeingcompared = currentNode;
                            while (FindCost(nodeBeingcompared.state) >= cost && index > 0)
                            {
                                --index;
                                nodeBeingcompared = nodeQueue[index];
                            }
                            nodeQueue.Insert(index, newNode);
                        }
                    }
                }
            }
        }

        private int FindCost(Vector2Int state)
        {
            int c = 1000000;
            foreach (Vector2Int goal in env.goals)
            {
                int thisCost = ObliqueDist(state, goal);
                if (thisCost < c)
                    c = thisCost;
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
        }

        protected override int LogMemory()
        {
            return nodeQueue.Count + triedNodes.Count;
            Console.WriteLine("End Memory Used: " + nodeQueue.Count + " Nodes in queue, " + triedNodes.Count + " Nodes in list = " + (nodeQueue.Count + triedNodes.Count));
        }
    }

}
