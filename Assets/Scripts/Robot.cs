using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    [HideInInspector]
    public Environment env;
    public LineRenderer line;

    //protected RectTransform Rep;

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

    public void Init(Environment Env)
    {
        BuildDebug.Log("Initializing " + name);
        SetDeltas();
        env = Env;
        initState = env.startState;

        transform.position = env.CellRepPos(initState);
        transform.localScale = new Vector2(Env.cellSize, Env.cellSize);
    }

    public virtual void FindSolution() { }

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
        BuildDebug.Log(name + ": Beginning Update Search");
        loops = 0;
        path.Clear();
        line.positionCount = 0;
        realTimeSolutionStarted = true;
        BreadcrumbManager.singleton.ResetCrumbs();
    }

    protected void BaseEndUS()
    {
        BuildDebug.Log("--------------------");
        BuildDebug.Log(name + ": Ending Update Search");
        BuildDebug.Log("Number of loops: " + loops);
        Debug.Log("About to Log path length");
        BuildDebug.Log("Path Length: " + path.Count);
        LogMemory();
        BuildDebug.Log("--------------------");
        realTimeSolutionStarted = false;
    }

    public void SilentEndUS()
    {
        realTimeSolutionStarted = false;
    }

    protected virtual void StartUpdateSolution() { }

    protected virtual void CancelUpdateSolution() { }

    protected virtual void StepUpdateSolution() { }

    protected virtual void CreateUpdatePath() { }

    protected void DropBreadcrumb(Vector3 pos)
    {

    }

    protected virtual void GenerateLineVisual()
    {
        //Generate the Line visual.
        line.positionCount = path.Count;
        Vector3 origin = new Vector3(virtualState.x, -virtualState.y);
        int index = line.positionCount;
        foreach (Node node in path)
        {
            --index;
            line.SetPosition(index, new Vector3(node.state.x, -node.state.y) - origin);
        }
    }

    protected virtual void LogMemory()
    {
        BuildDebug.Log("End Memory Used: Unknown");
    }

    public static int ObliqueDist(Vector2Int start, Vector2Int end)
    {
        return (Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y));
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
    }
}
