using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Uninform Search which expands each level one by one. </summary>
    public class BOT_BreadthFirst : Robot
    {
        /// <summary> Tree of all nodes including the unsearched frontier. </summary>
        protected List<Node> BFSNodes = new List<Node>();

        public BOT_BreadthFirst(Environment Env) : base(Env) { name = "Breadth First"; }

        /// <summary> Initialise the search. Reset lists and values, and add start node to tree. </summary>
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

        /// <summary> Do one iteration of the algorithm. Main Logic here. </summary>
        protected override void Step()
        {
            ++loops;
            if (nodeIndex >= BFSNodes.Count)
                end = true;
            else
            {
                //Get the next node to be searched from the tree.
                Node currentNode = BFSNodes[nodeIndex];
                cost = currentNode.level + 1;

                //End the search if the cell is a goal, or add nodes if it's free.
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
                    //Add the cells neighbors to the tree if they aren't already on it.
                    foreach (Vector2Int delta in deltas)
                    {
                        Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                        if (BFSNodes.Find(X => X.state == newNode.state) == null)
                            BFSNodes.Add(newNode);
                    }
                }
                ++nodeIndex; //Increment the index of the node beign searched.
            }
        }

        /// <summary> Compile the tree into a path by tracing back the parent of each node. </summary>
        protected override void CreateUpdatePath()
        {
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
        }
    }

}
