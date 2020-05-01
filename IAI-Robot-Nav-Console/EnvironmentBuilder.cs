using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IAI_Robot_Nav
{
    static class EnvironmentBuilder
    {
        public static string BuildEnvironment()
        {
            Console.Write("Directory of Environment: ");
            string dir = Console.ReadLine();

            //if (!Directory.Exists(dir))
            //    return;
            if (File.Exists(dir))
                Console.WriteLine("Overwriting File");

            Console.Write("Number of rows: ");
            Vector2Int gridSize = Vector2Int.zero;
            gridSize.y = int.Parse(Console.ReadLine());
            Console.Write("Number of columns: ");
            gridSize.x = int.Parse(Console.ReadLine());
            Console.WriteLine("Gridsize = " + gridSize);

            Console.WriteLine("Draw Environment: O = start, X = goal, L = wall, any other char = free");
            Console.Write("%|");
            for (int i = 0; i < gridSize.x; ++i)
                Console.Write(i);
            Console.WriteLine();
            CellState[,] grid = new CellState[gridSize.x, gridSize.y];
            Vector2Int start = Vector2Int.zero;
            List<Vector2Int> goals = new List<Vector2Int>();
            List<Rect> walls = new List<Rect>();
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

            StreamWriter SW = new StreamWriter(dir, false);
            //GridSize
            SW.WriteLine(gridSize);
            //Start Pos
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

        private static string V2str(Vector2Int val)
        {
            return "(" + val.x + ", " + val.y + ")";
        }

        private static string Rectstr(Rect val)
        {
            return "(" + val.x + ", " + val.y + ", " + val.width + ", " + val.height + ")";
        }
    }
}
