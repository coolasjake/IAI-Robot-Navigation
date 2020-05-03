using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IAI_Robot_Nav
{
    /// <summary> Class used to handle getting console input from the user to generate a custom environment.
    /// Saves the new environment as a text file which must then be interpreted to use. </summary>
    static class EnvironmentBuilder
    {
        /// <summary> Uses input to build an environment. Returns the directory of the new environment. </summary>
        public static string BuildEnvironment()
        {
            Console.Write("Directory of Environment: ");
            string dir = Console.ReadLine();
            
            //Report if the file already exists (will also terminate if directory doesn't exist).
            if (File.Exists(dir))
                Console.WriteLine("Overwriting File");

            //Take options from the user.
            Console.Write("Number of rows: ");
            Vector2Int gridSize = Vector2Int.zero;
            gridSize.y = int.Parse(Console.ReadLine());
            Console.Write("Number of columns: ");
            gridSize.x = int.Parse(Console.ReadLine());
            Console.WriteLine("Gridsize = " + gridSize);

            //Initialize values.
            CellState[,] grid = new CellState[gridSize.x, gridSize.y];
            Vector2Int start = Vector2Int.zero;
            List<Vector2Int> goals = new List<Vector2Int>();
            List<Rect> walls = new List<Rect>();

            //Get input about the Env from the user.
            Console.WriteLine("Draw Environment: O = start, X = goal, L = wall, any other char = free");
            //Draw column numbers.
            Console.Write("%|");
            for (int i = 0; i < gridSize.x; ++i)
                Console.Write(i);
            Console.WriteLine();
            //Begin recieving and interpreting cell inputs.
            for (int y = 0; y < gridSize.y; ++y)
            {
                Console.Write(y + "|");
                for (int x = 0; x < gridSize.x; ++x)
                {
                    char c = Console.ReadKey().KeyChar;
                    if (c == 'L' || c == 'l')
                    {
                        grid[x, y] = CellState.Wall;
                        walls.Add(new Rect(x, y, 1, 1));
                    }
                    else if (c == 'O' || c == 'o')
                    {
                        grid[x, y] = CellState.Robot;
                        start = new Vector2Int(0, 0);
                    }
                    else if (c == 'X' || c == 'x')
                    {
                        grid[x, y] = CellState.Goal;
                        goals.Add(new Vector2Int(x, y));
                    }
                    else
                        grid[x, y] = CellState.Free;
                }
                Console.WriteLine();
            }

            //Save the file
            StreamWriter SW = new StreamWriter(dir, false);

            SW.WriteLine(gridSize);
            SW.WriteLine(V2str(start));
            //Goals
            if (goals.Count > 0)
            {
                for (int i = 0; i < goals.Count - 1; ++i)
                {
                    SW.Write(V2str(goals[i]));
                    SW.Write(" | ");
                }
                SW.Write(V2str(goals[goals.Count - 1]));
                SW.WriteLine();
            }
            //Walls (as single cells)
            if (walls.Count > 0)
            {
                foreach (Rect w in walls)
                    SW.WriteLine(Rectstr(w));
            }

            SW.Close();

            return dir;
        }

        /// <summary> Returns the vector with round brackets '()' required by the interpreter. </summary>
        private static string V2str(Vector2Int val)
        {
            return "(" + val.x + ", " + val.y + ")";
        }

        /// <summary> Returns the rect with round brackets '()' required by the interpreter. </summary>
        private static string Rectstr(Rect val)
        {
            return "(" + val.x + ", " + val.y + ", " + val.width + ", " + val.height + ")";
        }
    }
}
