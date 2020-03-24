using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Environment : MonoBehaviour
{
    public RawEnvironment Env = new RawEnvironment();
    public GameObject[,] cells = new GameObject[0, 0];
    public GameObject cellPre;
    private float cellSize = 80;
    private Canvas canvas;

    private GameObject Cell(Vector2Int coords)
    {
        return cells[coords.x, coords.y];
    }

    void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
        Env = Save.LoadEnvironment("Assets/RobotNav-test.txt");
        GenerateEnvVisuals();
    }

    private void GenerateEnvVisuals()
    {
        cells = new GameObject[Env.gridSize.y, Env.gridSize.x];
        Vector2 startPos = new Vector2(cellSize, cellSize + cellSize * Env.gridSize.x);
        for (int row = 0; row < Env.gridSize.y; ++row)
        {
            for (int col = 0; col < Env.gridSize.x; ++col)
            {
                cells[row, col] = Instantiate(cellPre, new Vector3(startPos.x + (row * cellSize), startPos.y - (col * cellSize)), new Quaternion(), canvas.transform);
                cells[row, col].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
            }
        }

        Cell(Env.startPos).GetComponent<Image>().color = Color.red;

        foreach (Vector2Int goal in Env.goalsPos)
            Cell(goal).GetComponent<Image>().color = Color.green;

        foreach (Rect wall in Env.wallRects)
        {
            int i = 0;
            for (int x = (int)wall.x; i < (int)wall.width; ++x)
            {
                ++i;
                int j = 0;
                for (int y = (int)wall.y; j < (int)wall.height; ++y)
                {
                    ++j;
                    cells[x, y].GetComponent<Image>().color = Color.black;
                }
            }
        }
    }
}
