using System;
using System.Collections.Generic;
using System.Text;

namespace IAI_Robot_Nav
{
    /// <summary> Interface for the Bots to gain information about the environment through. </summary>
    public class Environment
    {
        public RawEnvironment rawEnv = new RawEnvironment();
        public CellState[,] cells = new CellState[0, 0];
        public Vector2Int startState = Vector2Int.zero;
        public float cellSize = 80;
        public List<Vector2Int> goals = new List<Vector2Int>();

        /// <summary> Gets the cell a Vector2Int represents. No error checking. </summary>
        private CellState Cell(Vector2Int coords)
        {
            return cells[coords.x, coords.y];
        }

        /// <summary> Sets a cell using a Vector2Int. No error checking. </summary>
        private void SetCell(Vector2Int coords, CellState state)
        {
            cells[coords.x, coords.y] = state;
        }

        /// <summary> Creates the environment using 'FileToLoad' or default directory. </summary>
        public Environment()
        {
            string EnvDir = Save.GetDirectory();
            rawEnv = Save.LoadEnvironment(EnvDir);
            GenerateGrid();
            PrintEnvironment();
        }

        /// <summary> Creates the environment using the given directory. </summary>
        public Environment(string directory)
        {
            rawEnv = Save.LoadEnvironment(directory);
            GenerateGrid();
        }

        /// <summary> Extracts information from the RawEnvironment and uses it to generate a grid,
        /// as well as quick references for the start and goals. </summary>
        private void GenerateGrid()
        {
            //Initialize the grid.
            cells = new CellState[rawEnv.gridSize.x, rawEnv.gridSize.y];
            for (int row = 0; row < rawEnv.gridSize.x; ++row)
            {
                for (int col = 0; col < rawEnv.gridSize.y; ++col)
                {
                    cells[row, col] = CellState.Free;
                }
            }

            //Add the walls first so important cells aren't overwritten.
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

            //Add the start (aka Robot), and save it in the quick reference variable.
            SetCell(rawEnv.startPos, CellState.Robot);
            startState = rawEnv.startPos;

            //Add each goal, and save them in the quick reference list.
            foreach (Vector2Int goal in rawEnv.goalsPos)
            {
                goals.Add(goal);
                SetCell(goal, CellState.Goal);
            }
        }

        /// <summary> Get the 'state' of a cell. Returns null state for OOB. </summary>
        public CellState GetCellState(Vector2Int cell)
        {
            if (cell.Outside(Vector2Int.zero, rawEnv.gridSize - new Vector2Int(1, 1)))
                return CellState.Null;

            return cells[cell.x, cell.y];
        }

        /// <summary> Print a representation of the environment using normal text characters. </summary>
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

    /// <summary> Possible states for each cell of the Environment. </summary>
    public enum CellState
    {
        Free,
        Robot,
        Wall,
        Goal,
        Null
    }
    
    /// <summary> Data structure which stores information about the Environment as Vector2Ints and Rects.
    /// TBF: currently the only place the grids size is stored. </summary>
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


