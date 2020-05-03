using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Depth-First Uninformed Search.
    /// Searches the tree by choosing the deepest node first, and backtracking if the path fails. </summary>
    public class BOT_DepthFirst : Robot
    {
        /// <summary> List of used nodes to prevent repeat checks. Effectively the 'tree' of this search. </summary>
        protected List<Node> usedNodes = new List<Node>();

        public BOT_DepthFirst(Environment Env) : base(Env) { name = "Depth First"; }

        /// <summary> Initialise the search. Reset lists and values, and add start node to tree. </summary>
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

        /// <summary> Do one iteration of the algorithm. Main Logic here. </summary>
        protected override void Step()
        {
            ++loops;
            if (nodeIndex >= path.Count || nodeIndex < 0)
                end = true;
            else
            {
                //Get the next node to check
                Node currentNode = path[nodeIndex];
                cost = currentNode.level + 1;

                //Get the type of this cell, and either finish the search at a goal, or continue search for a free cell.
                CellState cell = env.GetCellState(currentNode.state);
                bool endOfBranch = true;
                if (cell == CellState.Goal)
                {
                    //Signal that the search has succeded.
                    firstGoal = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                    end = true;
                    foundGoal = true;
                    return;
                }
                else if (cell == CellState.Free || cell == CellState.Robot)
                {
                    //Check each neghbor of this node (in the arbitrary order) and queue the first valid one.
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

                //If there were no options to search, remove the node from the current branch (path), and set the index to search to it's parent.
                //Otherwise increment the node index.
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
        }
    }


}
