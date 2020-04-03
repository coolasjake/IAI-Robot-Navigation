using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [HideInInspector]
    public Environment env;
    public GameObject line;

    protected RectTransform Rep;

    protected Vector2Int state;

    protected List<Vector2Int> deltas = new List<Vector2Int>();

    protected bool end = false;
    public int MaxLoops = 3000;
    protected int loops = 0;
    public int stepDelay = 3;
    private int currentDelay = 0;
    private bool realTimeSolutionStarted = false;

    private void SetDeltas()
    {
        deltas.Add(new Vector2Int(0, -1));
        deltas.Add(new Vector2Int(-1, 0));
        deltas.Add(new Vector2Int(0, 1));
        deltas.Add(new Vector2Int(1, 0));
    }

    public void Init(Environment Env)
    {
        if (line == null)
            line = Resources.Load<GameObject>("Prefabs/PathSegment");

        Rep = (RectTransform)transform;

        SetDeltas();
        env = Env;
        state = env.startState;

        Rep.position = env.CellRepPos(state);
        Rep.sizeDelta = new Vector2(Env.cellSize, Env.cellSize);
    }

    public virtual void FindSolution() { }

    public void UpdateSolutionControl(bool keyPressed)
    {
        if (realTimeSolutionStarted == false)
        {
            if (keyPressed)
            {
                StartUpdateSolution();
            }
        }
        else
        {
            if (currentDelay == stepDelay)
            {
                currentDelay = 0;
                if (!end && loops < MaxLoops)
                    StepUpdateSolution();
                else
                    CreateUpdatePath();
            }
            else
                ++currentDelay;
        }
    }

    protected RectTransform CreatePathSegment(Vector2 start, Vector2 end)
    {
        GameObject GO = Instantiate(line);
        ((RectTransform)GO.transform).anchoredPosition = start - new Vector2(5, 5);
        ((RectTransform)GO.transform).localScale = end - start + new Vector2(5, 5);
        return (RectTransform)GO.transform;
    }

    protected void BaseStartUS()
    {
        Debug.Log("Beginning Update Search");
        loops = 0;
        realTimeSolutionStarted = true;
    }

    protected void BaseEndUS()
    {
        Debug.Log("Ending Update Search");
        realTimeSolutionStarted = false;
    }

    protected virtual void StartUpdateSolution() { }

    protected virtual void StepUpdateSolution() { }

    protected virtual void CreateUpdatePath() { }

    public enum MoveDir
    {
        Up,
        Left,
        Down,
        Right
    }

    [System.Serializable]
    public struct Node
    {
        public bool exists;
        public Vector2Int state;
        public Vector2Int actionUsed;
        public int level;
        public int parentIndex;

        public Node(Vector2Int NewState, Vector2Int Action, int Level, int ParentIndex)
        {
            exists = true;
            state = NewState;
            actionUsed = Action;
            level = Level;
            parentIndex = ParentIndex;
        }
    }
}
