using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Custom Informed Search.
    /// Performs simultaneous BFSs from the start and goal nodes, and connects the paths where they meet.</summary>
    public class BOT_Handshake : Robot
    {
        /// <summary> List of used nodes to prevent repeat checks. Frontier nodes are NOT duplicated here.
        /// This list is shared between both parts of the search.  </summary>
        protected List<Node> usedNodes = new List<Node>();
        /// <summary> List of discovered but unchecked nodes for the start-to-goal search.
        /// Nodes are added to 'usedNodes' AFTER being removed from here. </summary>
        public List<Node> startFrontier = new List<Node>();
        /// <summary> List of discovered but unchecked nodes for the goal-to-start search.
        /// Nodes are added to 'usedNodes' AFTER being removed from here. </summary>
        public List<Node> goalFrontier = new List<Node>();

        /// <summary> Current search index for the start-to-goal search. </summary>
        protected int startIndex = 0;
        /// <summary> Current search index for the goal-to-start search. </summary>
        protected int goalIndex = 0;
        /// <summary> The node where the start-to-goal meets the frontier of the other search.
        /// Needs to be seperate since both paths need an end point to be compiled from.</summary>
        protected Node startHandshake;
        /// <summary> The node where the goal-to-start meets the frontier of the other search.
        /// Needs to be seperate since both paths need an end point to be compiled from.</summary>
        protected Node goalHandshake;
        /// <summary> Value used to alternate between s-g and g-s searches.
        /// Not necessary here but kept for compatability with UI version. </summary>
        public bool doingStart = true;

        public BOT_Handshake(Environment Env) : base(Env) { name = "Handshake"; }

        /// <summary> Initialise the search. Reset lists and values, add start node to start frontier, and each goal to goal frontier. </summary>
        protected override void StartUpdateSolution()
        {
            BaseStartUS();
            
            cost = 0;
            nodeIndex = 0;
            startIndex = 0;
            goalIndex = 0;
            end = false;
            foundGoal = false;
            path.Clear();
            usedNodes.Clear();
            startFrontier.Clear();
            goalFrontier.Clear();
            startFrontier.Add(new Node(initState, Vector2Int.zero, cost, -1));
            foreach (Vector2Int goal in env.goals)
                goalFrontier.Add(new Node(goal, Vector2Int.zero, cost, -1));
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
            //Alternate searches
            if (doingStart)
            {
                //Start frontier (start-to-goal)
                doingStart = false;
                ++loops;
                //If there are no nodes in either frontier there is no possible solution.
                if (startFrontier.Count <= 0)
                    end = true;
                else
                {
                    //Get the next node of the s-g frontier, and transfer it to the 'usedNodes' list.
                    Node currentNode = startFrontier[0];
                    startFrontier.RemoveAt(0);
                    nodeIndex = usedNodes.Count;
                    usedNodes.Add(currentNode);
                    cost = currentNode.level + 1;

                    //Check if the cell is a free space, then find if it exists in the other direction's frontier.
                    CellState cell = env.GetCellState(currentNode.state);
                    if (cell != CellState.Null && cell != CellState.Wall)
                        goalHandshake = goalFrontier.Find(X => X.state == currentNode.state);
                    else
                        goalHandshake = null;

                    //If the node does exist in the other frontier, complete the handshake.
                    if (goalHandshake != null)
                    {
                        //This value is needed to compile the other half of the path.
                        startHandshake = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                        end = true;
                        foundGoal = true;
                        return;
                    }
                    else if (cell == CellState.Free || cell == CellState.Robot)
                    {
                        //If the node does not complete a handshake, and is a free cell, add any unchecked neighbors to the frontier.
                        foreach (Vector2Int delta in deltas)
                        {
                            Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                            if (usedNodes.Find(X => X.state == newNode.state) == null && startFrontier.Find(X => X.state == newNode.state) == null)
                                startFrontier.Add(newNode);
                        }
                    }
                }
            }
            else
            {
                //Goal frontier (goal-to-start)
                doingStart = true;
                ++loops;
                //If there are no nodes in either frontier there is no possible solution.
                if (goalFrontier.Count <= 0)
                    end = true;
                else
                {
                    //Get the next node of the g-s frontier, and transfer it to the 'usedNodes' list.
                    Node currentNode = goalFrontier[0];
                    goalFrontier.RemoveAt(0);
                    nodeIndex = usedNodes.Count;
                    usedNodes.Add(currentNode);
                    cost = currentNode.level + 1;

                    //Check if the cell is a free space, then find if it exists in the other direction's frontier.
                    CellState cell = env.GetCellState(currentNode.state);
                    if (cell != CellState.Null && cell != CellState.Wall)
                        startHandshake = startFrontier.Find(X => X.state == currentNode.state);
                    else
                        startHandshake = null;

                    //If the node does exist in the other frontier, complete the handshake.
                    if (startHandshake != null)
                    {
                        //This value is needed to compile the other half of the path.
                        goalHandshake = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                        end = true;
                        foundGoal = true;
                        return;
                    }
                    else if (cell == CellState.Free || cell == CellState.Robot || cell == CellState.Goal)
                    {
                        //If the node does not complete a handshake, and is a free cell, add any unchecked neighbors to the frontier.
                        foreach (Vector2Int delta in deltas)
                        {
                            Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                            if (usedNodes.Find(X => X.state == newNode.state) == null && goalFrontier.Find(X => X.state == newNode.state) == null)
                                goalFrontier.Add(newNode);
                        }
                    }
                }
            }
        }

        /// <summary> Compile the handshake path by retracing the parent of each node in both parts of the search, and joining the two halves. </summary>
        protected override void CreateUpdatePath()
        {
            if (foundGoal)
            {
                //Insert nodes from the s-g search at the start of the list, so that they are in the correct order.
                Node nextNode = startHandshake;
                while (nextNode.parentIndex != -1)
                {
                    path.Insert(0, usedNodes[nextNode.parentIndex]);
                    nextNode = usedNodes[nextNode.parentIndex];
                }

                //Insert nodes from the g-s search at the end of the list, since the traceback will already be in the correct order.
                nextNode = goalHandshake;
                while (nextNode.parentIndex != -1)
                {
                    path.Add(usedNodes[nextNode.parentIndex]);
                    nextNode = usedNodes[nextNode.parentIndex];
                }
                firstGoal = path[path.Count - 1];
                virtualState = firstGoal.state;

                //Generate the actions used to get to each state by comparing to the previous state,
                //since the second half has the actions reversed by default.
                nextNode = path[0]; 
                for (int i = 1; i < path.Count; ++i)
                {
                    path[i].actionUsed = path[i].state - nextNode.state;
                    nextNode = path[i];
                }
            }

            BaseEndUS();
        }

        protected override int LogMemory()
        {
            return usedNodes.Count + goalFrontier.Count + startFrontier.Count;
        }
    }

}
