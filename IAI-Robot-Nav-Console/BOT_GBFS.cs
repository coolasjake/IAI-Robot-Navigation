using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Greedy Best-First Informed Search.
    /// Searches the tree by choosing the nodes closest to the goal first. </summary>
    public class BOT_GBFS : Robot
    {
        /// <summary> List of nodes the algorithm wants to search, in order of how close they are to a goal. </summary>
        protected List<Node> nodeQueue = new List<Node>();
        /// <summary> List of all nodes that have already been searched. </summary>
        protected List<Node> triedNodes = new List<Node>();

        public BOT_GBFS(Environment Env) : base(Env) { name = "GBFS"; }

        /// <summary> Initialise the search. Reset lists and values, and add start node to tree. </summary>
        protected override void StartUpdateSolution()
        {
            BaseStartUS();
            
            cost = 0;
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

        /// <summary> Do one iteration of the algorithm. Main Logic here. </summary>
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
                    //Console.WriteLine("Found goal: " + currentNode.state);
                    foundGoal = true;
                    return;
                }
                else if (cell == CellState.Free || cell == CellState.Robot)
                {
                    //For each neighbor of this node:
                    foreach (Vector2Int delta in deltas)
                    {
                        //Find the cost of the neighbor (before search so it can be added in the node class),
                        //and see if it has already been searched, or is in the queue.
                        cost = FindCost(currentNode.state + delta);
                        Node newNode = new Node(currentNode.state + delta, delta, cost, currentNode.level + 1, nodeIndex);
                        if (triedNodes.Find(X => X.state == newNode.state) == null && nodeQueue.Find(X => X.state == newNode.state) == null)
                        {
                            //If it is valid, iterate backwards through the list until an item with a cost less than the node is found,
                            //then add the node to the list in that position.
                            int index = nodeQueue.Count;
                            if (nodeQueue.Count > 0)
                            {
                                Node nodeBeingcompared = nodeQueue[index];
                                while (nodeBeingcompared.level >= cost && index > 0)
                                {
                                    --index;
                                    nodeBeingcompared = nodeQueue[index];
                                }
                            }
                            nodeQueue.Insert(index, newNode);
                        }
                    }
                }
            }
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
