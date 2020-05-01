using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class Robot
    {
        public Environment env;

        protected string name = "DEFAULT";

        protected bool foundGoal = false;
        protected Vector2Int initState;
        protected Vector2Int virtualState;
        protected Node firstGoal;
        protected int cost = 0;
        protected int nodeIndex = 0;

        protected List<Vector2Int> deltas = new List<Vector2Int>();
        public List<Node> path = new List<Node>();

        public bool moved = false;
        protected bool end = false;
        public int MaxLoops = 3000;
        protected int loops = 0;
        public int stepDelay = 3;
        public int currentDelay = 0;
        private bool realTimeSolutionStarted = false;

        private void SetDeltas()
        {
            deltas.Add(new Vector2Int(0, -1));  //Up
            deltas.Add(new Vector2Int(-1, 0));  //Left
            deltas.Add(new Vector2Int(0, 1));   //Down
            deltas.Add(new Vector2Int(1, 0));   //Right
        }

        public Robot(Environment Env)
        {
            SetDeltas();
            env = Env;
            initState = env.startState;
        }

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

        protected void BaseStartUS()
        {
            loops = 0;
            path.Clear();
            realTimeSolutionStarted = true;
        }

        protected void BaseEndUS()
        {
            LogMemory();
            realTimeSolutionStarted = false;
        }

        protected virtual void StartUpdateSolution() { }

        protected virtual void CancelUpdateSolution() { }

        protected virtual void StepUpdateSolution() { }

        protected virtual void CreateUpdatePath() { }

        protected virtual void Step() { }

        protected virtual int LogMemory() { return -1; }

        private void PrintPath()
        {
            foreach (Node n in path)
            {
                Console.WriteLine(n);
            }
            Console.WriteLine("Path Length = " + (path.Count - 1));
        }

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

        public static int ObliqueDist(Vector2Int start, Vector2Int end)
        {
            return (Math.Abs(start.x - end.x) + Math.Abs(start.y - end.y));
        }

        public enum MoveDir
        {
            Up,
            Left,
            Down,
            Right
        }

        [System.Serializable]
        public class Node
        {
            //public bool exists;
            public Vector2Int state;
            public Vector2Int actionUsed;
            public int cost;
            public int parentIndex;

            public Node(Vector2Int NewState, Vector2Int Action, int Level, int ParentIndex)
            {
                //exists = true;
                state = NewState;
                actionUsed = Action;
                cost = Level;
                parentIndex = ParentIndex;
            }

            public override string ToString()
            {
                return state + " using " + actionUsed + " from " + parentIndex;
            }
        }
    }
}