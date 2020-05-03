using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class BOT_AStar : Robot
    {
        protected List<Node> nodeQueue = new List<Node>();
        protected List<Node> triedNodes = new List<Node>();

        public BOT_AStar(Environment Env) : base(Env) { name = "A*"; }

        protected override void StartUpdateSolution()
        {
            BaseStartUS();
            
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
        }

        protected override void StepUpdateSolution()
        {
            Step();
        }

        protected override void Step()
        {
            ++loops;
            if (nodeQueue.Count == 0)
                end = true;
            else
            {
                //Get the next node in the queue, and transfer it to the 'triedNodes' list.
                Node currentNode = nodeQueue[0];
                triedNodes.Add(currentNode);
                nodeIndex = triedNodes.Count - 1;
                nodeQueue.RemoveAt(0);

                //Check if the node is a goal, otherwise if it is free add it's neighbors.
                CellState cell = env.GetCellState(currentNode.state);
                if (cell == CellState.Goal)
                {
                    firstGoal = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                    end = true;
                    foundGoal = true;
                    return;
                }
                else if (cell == CellState.Free || cell == CellState.Robot)
                {
                    //Discover each neighbor of the current cell.
                    foreach (Vector2Int delta in deltas)
                    {
                        cost = FindCost(currentNode.state + delta);
                        Node newNode = new Node(currentNode.state + delta, delta, currentNode.level + 1, cost, nodeIndex);

                        //Find any existing copies of this cell in the unsearched 'nodeQueue'.
                        Node existing = nodeQueue.Find(X => X.state == newNode.state);
                        if (existing != null)
                        {
                            //If a copy exists, and is worse than this node, delete it, otherwise skip the new node.
                            if (Heuristic(newNode) < Heuristic(existing))
                                nodeQueue.Remove(existing);
                            else
                                continue;
                        }

                        //If the node has NOT already been searched, add it to the queue just after the latest node with the same cost.
                        //NOTE: Something like the above 'replace existing' code would need to be done instead for a non-uniform-cost search.
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

        /// <summary> Shorthand to combine the level and cost of the node. </summary>
        private int Heuristic(Node n)
        {
            return n.cost + n.level;
        }

        /// <summary> Find the minimum ortogonal distance from the state to any goal. </summary>
        private int FindCost(Vector2Int state)
        {
            int c = 1000000;
            foreach (Vector2Int goal in env.goals)
            {
                int thisCost = OrthogonalDist(state, goal);
                if (thisCost < c)
                    c = thisCost;
            }
            return c;
        }

        /// <summary> Compile the tree into a path by tracing back the parent of each node. </summary>
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
        }
    }

}
