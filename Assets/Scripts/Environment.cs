using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environment : MonoBehaviour
{
    public RawEnvironment rawEnv = new RawEnvironment();
    public CellState[,] cells = new CellState[0, 0];
    public GameObject[,] cellsVis = new GameObject[0, 0];
    public GameObject cellPre;
    public Vector2Int startState = Vector2Int.zero;
    public float cellSize = 80;
    public Transform CellHolder;
    public List<Vector2Int> goals = new List<Vector2Int>(); 
    private Canvas canvas;

    private GameObject CellVis(Vector2Int coords)
    {
        return cellsVis[coords.x, coords.y];
    }

    private CellState Cell(Vector2Int coords)
    {
        return cells[coords.x, coords.y];
    }

    private void SetCell(Vector2Int coords, CellState state)
    {
        cells[coords.x, coords.y] = state;
    }

    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        BuildDebug.Log("Starting Load");
        string EnvDir = Save.GetDirectory();
        BuildDebug.Log("Loading Directory: " + EnvDir);
        rawEnv = Save.LoadEnvironment(EnvDir);
        GenerateGrid();
        GenerateEnvVisuals();
    }

    private void GenerateGrid()
    {
        cells = new CellState[rawEnv.gridSize.x, rawEnv.gridSize.y];
        Vector2 startPos = new Vector2(cellSize, cellSize + cellSize * rawEnv.gridSize.y);
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

        foreach (Vector2Int goal in rawEnv.goalsPos)
        {
            goals.Add(goal);
            SetCell(goal, CellState.Goal);
        }
    }

    private void GenerateEnvVisuals()
    {
        cellsVis = new GameObject[rawEnv.gridSize.x, rawEnv.gridSize.y];
        //Vector2 startPos = new Vector2(cellSize, cellSize + cellSize * rawEnv.gridSize.y);
        for (int row = 0; row < rawEnv.gridSize.x; ++row)
        {
            for (int col = 0; col < rawEnv.gridSize.y; ++col)
            {
                cellsVis[row, col] = Instantiate(cellPre, CellHolder.transform);
                cellsVis[row, col].transform.position = (Vector3)CellRepPos(row, col) + new Vector3(0, 0, 1);
                cellsVis[row, col].transform.localScale = new Vector2(cellSize, cellSize);
                cellsVis[row, col].GetComponent<WorldCell>().Coords = row + ", " + col;
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
                    cellsVis[x, y].GetComponent<SpriteRenderer>().color = Color.black;
                    cellsVis[x, y].GetComponent<WorldCell>().color = Color.white;
                }
            }
        }

        CellVis(rawEnv.startPos).GetComponent<SpriteRenderer>().color = Color.red;

        foreach (Vector2Int goal in rawEnv.goalsPos)
            CellVis(goal).GetComponent<SpriteRenderer>().color = Color.green;
    }

    public CellState GetCellState(Vector2Int cell)
    {
        if (cell.Outside(Vector2Int.zero, rawEnv.gridSize - new Vector2Int(1, 1)))
            return CellState.Null;

        return cells[cell.x, cell.y];
    }

    public Vector2 CellRepPos(int x, int y)
    {
        Vector2 startPos = new Vector2(-(rawEnv.gridSize.x * cellSize * 0.5f), (rawEnv.gridSize.y * cellSize * 0.5f)); //new Vector2(cellSize, - cellSize * 2); //* rawEnv.gridSize.y);
        return new Vector3(startPos.x + (x * cellSize), startPos.y - (y * cellSize));
    }

    public Vector2 CellRepPos(Vector2Int pos)
    {
        return CellRepPos(pos.x, pos.y);
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
    public Vector2Int gridSize = Vector2Int.zero;
    public Vector2Int startPos = Vector2Int.zero;
    public List<Vector2Int> goalsPos = new List<Vector2Int>();
    public List<Rect> wallRects = new List<Rect>();
}
