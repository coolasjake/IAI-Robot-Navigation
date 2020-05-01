using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    public class Environment
    {
        public RawEnvironment rawEnv = new RawEnvironment();
        public CellState[,] cells = new CellState[0, 0];
        public Vector2Int startState = Vector2Int.zero;
        public float cellSize = 80;
        public List<Vector2Int> goals = new List<Vector2Int>();

        private CellState Cell(Vector2Int coords)
        {
            return cells[coords.x, coords.y];
        }

        private void SetCell(Vector2Int coords, CellState state)
        {
            cells[coords.x, coords.y] = state;
        }

        /// <summary> Creates the environment using default directories. </summary>
        public Environment()
        {
            string EnvDir = Save.GetDirectory();
            //Console.WriteLine("Loading Directory: " + EnvDir);
            rawEnv = Save.LoadEnvironment(EnvDir);
            GenerateGrid();
            PrintEnvironment();
        }

        /// <summary> Creates the environment using default directories. </summary>
        public Environment(string directory)
        {
            rawEnv = Save.LoadEnvironment(directory);
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            //Console.WriteLine("Generating Grid");
            //Console.WriteLine("Grid Size: " + rawEnv.gridSize.x + ", " + rawEnv.gridSize.y);
            cells = new CellState[rawEnv.gridSize.x, rawEnv.gridSize.y];
            Vector2Int startPos = new Vector2Int(cellSize, cellSize + cellSize * rawEnv.gridSize.y);
            for (int row = 0; row < rawEnv.gridSize.x; ++row)
            {
                for (int col = 0; col < rawEnv.gridSize.y; ++col)
                {
                    cells[row, col] = CellState.Free;
                }
            }

            foreach (Rect wall in rawEnv.wallRects)
            {
                int i = 0;
                for (int x = (int)wall.x; i < (int)wall.width; ++x)
                {
                    ++i;
                    int j = 0;
                    for (int y = (int)wall.y; j < (int)wall.height; ++y)
                    {
                        ++j;
                        cells[x, y] = CellState.Wall;
                    }
                }
            }

            SetCell(rawEnv.startPos, CellState.Robot);
            startState = rawEnv.startPos;
            //Console.WriteLine("Start pos: " + startState);

            foreach (Vector2Int goal in rawEnv.goalsPos)
            {
                goals.Add(goal);
                SetCell(goal, CellState.Goal);
            }
        }

        public CellState GetCellState(Vector2Int cell)
        {
            if (cell.Outside(Vector2Int.zero, rawEnv.gridSize - new Vector2Int(1, 1)))
                return CellState.Null;

            return cells[cell.x, cell.y];
        }

        public void PrintEnvironment()
        {
            for (int y = 0; y < rawEnv.gridSize.y; ++y)
            {
                string line = "";
                for (int x = 0; x < rawEnv.gridSize.x; ++x)
                {
                    CellState c = cells[x, y];
                    if (c == CellState.Free)
                        line = line + ".";
                    else if (c == CellState.Goal)
                        line = line + "X";
                    else if (c == CellState.Wall)
                        line = line + "L";
                    else if (c == CellState.Robot)
                        line = line + "O";
                }
                Console.WriteLine(line);
            }
        }
    }

    public enum CellState
    {
        Free,
        Robot,
        Wall,
        Goal,
        Null
    }

    [System.Serializable]
    public class RawEnvironment
    {
        public string name = "unknown";
        public Vector2Int gridSize = Vector2Int.zero;
        public Vector2Int startPos = Vector2Int.zero;
        public List<Vector2Int> goalsPos = new List<Vector2Int>();
        public List<Rect> wallRects = new List<Rect>();
    }
}


