using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Parent class for the Bots which allows them to be interfaced with easily.
    /// Child Bots use parent values such as 'path' list, and implement search using 'step' and 'startUpdateSolution' functions
    /// to make interacting with Unity for the UI version easier. </summary>
    public abstract class Robot
    {
        /// <summary> A reference to the environment this bot is searching. Bots are trusted with complete data. </summary>
        public Environment env;

        /// <summary> Name of the bot for console (and in future data-log) output. </summary>
        protected string name = "DEFAULT";

        /// <summary> List of possible directions as V2Is, in the default order, for easy iteration. </summary>
        protected List<Vector2Int> deltas = new List<Vector2Int>();
        
        /// <summary> The path that the Bot compiles at the end of its search. Required my multiple functions. </summary>
        public List<Node> path = new List<Node>();

        //Variables that many different bots might use. Only 'foundGoal' is required to be used.
        protected bool foundGoal = false;
        protected Vector2Int initState;
        protected Vector2Int virtualState;
        protected Node firstGoal;
        protected int cost = 0;
        protected int nodeIndex = 0;
        protected int loops = 0;

        //Variables used to control real time iteration in Visual solution. Not used here, but kept to make transfers easier.
        public bool moved = false;
        protected bool end = false;
        public int MaxLoops = 3000;
        public int stepDelay = 3;
        public int currentDelay = 0;
        private bool realTimeSolutionStarted = false;

        /// <summary> Generates the ordered list of directions. </summary>
        private void SetDeltas()
        {
            deltas.Add(new Vector2Int(0, -1));  //Up
            deltas.Add(new Vector2Int(-1, 0));  //Left
            deltas.Add(new Vector2Int(0, 1));   //Down
            deltas.Add(new Vector2Int(1, 0));   //Right
        }

        /// <summary> Create a default robot using the given environment. </summary>
        public Robot(Environment Env)
        {
            SetDeltas();
            env = Env;
            initState = env.startState;
        }

        /// <summary> Run steps necessary to find solution, then print with documentation and extra data. </summary>
        public void PFindSolution()
        {
            Console.WriteLine("--- STARTING ROBOT NAV SOLUTION ---");
            Console.WriteLine("Beginning Search using << " + name + " >>");
            StartUpdateSolution();
            while (!end && loops < MaxLoops)
            {
                Step();
            }

            if (foundGoal)
            {
                Console.WriteLine("Path Found:");
                CreateUpdatePath();
                PrintPathDirections();
            }
            else
                Console.WriteLine("Path Not Found.");
            Console.WriteLine(loops + " iterations.");

            BaseEndUS();
            Console.WriteLine("-----------");
        }

        /// <summary> Run steps necessary to find solution, then print in the format required by the assignment. </summary>
        public void FindSolution()
        {
            StartUpdateSolution();
            while (!end && loops < MaxLoops)
            {
                Step();
            }

            Console.WriteLine(env.rawEnv.name + " " + name + " " + LogMemory());

            if (foundGoal)
            {
                CreateUpdatePath();
                PrintPathDirections();
            }
            else
                Console.WriteLine("No solution found.");

            BaseEndUS();
        }

        /// <summary> Used by the UI version to control real time searches. Not used in console version. </summary>
        public void UpdateSolutionControl(bool keyPressed)
        {
            if (realTimeSolutionStarted == false)
            {
                if (keyPressed)
                    StartUpdateSolution();
            }
            else
            {
                if (keyPressed)
                    CancelUpdateSolution();
                else if (currentDelay >= stepDelay)
                {
                    while (currentDelay >= stepDelay)
                    {
                        --currentDelay;
                        if (!end && loops < MaxLoops)
                        {
                            moved = true;
                            StepUpdateSolution();
                        }
                        else
                        {
                            CreateUpdatePath();
                            break;
                        }
                    }
                    currentDelay = 0;
                }
                else
                    ++currentDelay;
            }
        }

        /// <summary> Perform the actions that all searches need to do before beginning a search. (logs removed for assignment) </summary>
        protected void BaseStartUS()
        {
            loops = 0;
            path.Clear();
            realTimeSolutionStarted = true;
        }

        /// <summary> Perform the actions that all searches need to do after ending a search.
        /// (logs other than memory removed for assignment) </summary>
        protected void BaseEndUS()
        {
            LogMemory();
            realTimeSolutionStarted = false;
        }

        /// <summary> Instruct the inherited Bot to initialize the search. </summary>
        protected virtual void StartUpdateSolution() { }

        /// <summary> Instruct the inherited Bot to cancel the search. (only used by UI version) </summary>
        protected virtual void CancelUpdateSolution() { }

        /// <summary> Instruct the inherited Bot to do one iteration of the search, including updates to UI. </summary>
        protected virtual void StepUpdateSolution() { Step(); }

        /// <summary> Instruct the inherited Bot to generate a path using the explored tree. </summary>
        protected virtual void CreateUpdatePath() { }

        /// <summary> Instruct the inherited Bot to do one iteration of the search. (called by StepUpdateSolution() in UI version) </summary>
        protected abstract void Step();

        /// <summary> Instruct the inherited Bot to report how many nodes it had in the generated tree. </summary>
        protected virtual int LogMemory() { return -1; }

        /// <summary> Print the path using the state/coordinates of each node. </summary>
        private void PrintPath()
        {
            foreach (Node n in path)
            {
                Console.WriteLine(n);
            }
            Console.WriteLine("Path Length = " + (path.Count - 1));
        }

        /// <summary> Print the path using directions in english. (Assignment format) </summary>
        private void PrintPathDirections()
        {
            //Console.WriteLine("Printing Path Directions:");
            for (int i = 1; i < path.Count; ++i)
            {
                Console.Write(Dir(path[i].actionUsed));
            }
            Console.WriteLine();
            Console.WriteLine("Path Length = " + (path.Count - 1));
        }

        /// <summary> Convert a V2I direction into a text direction. </summary>
        private string Dir(Vector2Int delta)
        {
            if (delta.y < 0)
                return "Up; ";
            if (delta.y > 0)
                return "Down; ";
            if (delta.x < 0)
                return "Left; ";
            if (delta.x > 0)
                return "Right; ";
            return "IDLE; ";
        }

        /// <summary> Calculate the minimum number of moves from coords A to coords B. Used by A* and GBFS. </summary>
        public static int OrthogonalDist(Vector2Int start, Vector2Int end)
        {
            return (Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y));
        }

        /// <summary> Enum representing the possible directions in order. (unused) </summary>
        public enum MoveDir
        {
            Up,
            Left,
            Down,
            Right
        }
        
        /// <summary> Generic node used by all algorithms.
        /// Stores the coords (state) of the current cell,
        /// the action used to reach it (as V2I),
        /// the cost / level of the node (cost),
        /// and the index of the parent node. </summary>
        [System.Serializable]
        public class Node
        {
            public Vector2Int state;
            public Vector2Int actionUsed;
            public int cost;
            public int level;
            public int parentIndex;

            public Node(Vector2Int NewState, Vector2Int Action, int Level, int ParentIndex)
            {
                state = NewState;
                actionUsed = Action;
                level = Level;
                parentIndex = ParentIndex;
            }

            public Node(Vector2Int NewState, Vector2Int Action, int Level, int Cost, int ParentIndex)
            {
                state = NewState;
                actionUsed = Action;
                level = Level;
                cost = Cost;
                parentIndex = ParentIndex;
            }

            /// <summary> Returns the state, action used and parent index of the node as a string for debugging. </summary>
            public override string ToString()
            {
                return state + " using " + actionUsed + " from " + parentIndex;
            }
        }
    }
}