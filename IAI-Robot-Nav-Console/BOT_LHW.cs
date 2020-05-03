using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Custom Uninformed Search.
    /// Similar to DFS but always tries nodes to the left and then clockwise in order to 'follow the wall'. </summary>
    public class BOT_LHW : Robot
    {
        /// <summary> List of used nodes to prevent repeat checks. Effectively the 'tree' of this search. </summary>
        protected List<Node> usedNodes = new List<Node>();
        
        /// <summary> Initialise the bot. Calls base then changes name. </summary>
        public BOT_LHW(Environment Env) : base(Env) { name = "Wall Follower"; }

        /// <summary> Initialise the search. Reset lists and values, and add start node to tree. </summary>
        protected override void StartUpdateSolution()
        {
            BaseStartUS();
            
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
            //End search if no unsearched nodes are left.
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
                    //Get the node to the left (relative to last move)
                    Vector2Int delta = ClockwiseDelta(currentNode.actionUsed, true);
                    for (int i = 0; i < 3; ++i)
                    {
                        //Find if the node has already been searched, and check the node clockwise if it has.
                        Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                        if (usedNodes.Find(X => X.state == currentNode.state + delta) == null)
                        {
                            //If a node has not been checked, add it to the end of the branch (path) to be checked next.
                            usedNodes.Add(newNode);
                            path.Add(newNode);
                            endOfBranch = false;
                            break;
                        }
                        delta = ClockwiseDelta(delta);
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

        /// <summary> Get the direction (as V2I) clockwise of the given direction. </summary>
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

        /// <summary> Get the direction (as V2I) either clockwise or ant-clockwise of the given direction, based on the value of 'reversed'.
        /// True = Anti-clockwise and False = Clockwise. </summary>
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
            //For DFS and LHWS the path is the same as the 'current branch'.
        }

        protected override int LogMemory()
        {
            return usedNodes.Count;
        }
    }


}
