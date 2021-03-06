﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOT_Handshake : Robot
{
    protected List<Node> usedNodes = new List<Node>();
    public List<Node> startFrontier = new List<Node>();
    public List<Node> goalFrontier = new List<Node>();
    protected int startIndex = 0;
    protected int goalIndex = 0;
    protected Node startHandshake;
    protected Node goalHandshake;
    public bool doingStart = true;

    public override void FindSolution()
    {
        Debug.Log("Beginning Search");
        int loops = 0;
        int level = 0;
        //int nodeIndex = 0;
        bool end = false;
        bool foundGoal = false;
        usedNodes.Add(new Node(initState, Vector2Int.zero, level, -1));
        while (!end && loops < MaxLoops)
        {
            Step();
        }

        //If the goal was found, compile the tree into a path.
        if (foundGoal)
        {
            Node nextNode = usedNodes[nodeIndex];
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, usedNodes[nextNode.parentIndex]);
                nextNode = usedNodes[nextNode.parentIndex];
            }
        }
    }

    protected override void StartUpdateSolution()
    {
        BaseStartUS();

        Debug.Log("Setting Variables");
        cost = 0;
        nodeIndex = 0;
        startIndex = 0;
        goalIndex = 0;
        end = false;
        foundGoal = false;
        path.Clear();
        usedNodes.Clear();
        startFrontier.Clear();
        goalFrontier.Clear();
        startFrontier.Add(new Node(initState, Vector2Int.zero, cost, -1));
        foreach (Vector2Int goal in env.goals)
            goalFrontier.Add(new Node(goal, Vector2Int.zero, cost, -1));
    }

    protected override void CancelUpdateSolution()
    {
        BaseEndUS();

        Debug.Log("Cancelling Depth First");
    }

    protected override void StepUpdateSolution()
    {
        Step();
    }

    private void Step()
    {
        if (doingStart)
        {
            doingStart = false;
            ++loops;
            //Start frontier
            if (startFrontier.Count <= 0)
            {
                end = true;
                //Console.WriteLine("Search ended without finding solution. Start");
            }
            else
            {
                Node currentNode = startFrontier[0];
                startFrontier.RemoveAt(0);
                virtualState = currentNode.state;
                nodeIndex = usedNodes.Count;
                usedNodes.Add(currentNode);
                transform.position = env.CellRepPos(currentNode.state);
                cost = currentNode.level + 1;
                CellState cell = env.GetCellState(currentNode.state);
                if (cell != CellState.Null && cell != CellState.Wall)
                    goalHandshake = goalFrontier.Find(X => X.state == currentNode.state);
                else
                    goalHandshake = null;
                if (goalHandshake != null)
                {
                    //First goal is the handshake at this point in the algorithm.
                    startHandshake = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                    end = true;
                    //Console.WriteLine("Found handshake: " + currentNode.state);
                    foundGoal = true;
                    return;
                }
                else if (cell == CellState.Free || cell == CellState.Robot)
                {
                    foreach (Vector2Int delta in deltas)
                    {
                        Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                        if (usedNodes.Find(X => X.state == newNode.state) == null && startFrontier.Find(X => X.state == newNode.state) == null)
                            startFrontier.Add(newNode);
                    }
                }
            }
        }
        else
        {
            doingStart = true;
            ++loops;
            //Goal frontier
            if (goalFrontier.Count <= 0)
            {
                end = true;
                //Console.WriteLine("Search ended without finding solution. Goal");
            }
            else
            {
                Node currentNode = goalFrontier[0];
                goalFrontier.RemoveAt(0);
                nodeIndex = usedNodes.Count;
                usedNodes.Add(currentNode);
                virtualState = currentNode.state;
                transform.position = env.CellRepPos(currentNode.state);
                cost = currentNode.level + 1;
                CellState cell = env.GetCellState(currentNode.state);
                if (cell != CellState.Null && cell != CellState.Wall)
                    startHandshake = startFrontier.Find(X => X.state == currentNode.state);
                else
                    startHandshake = null;
                if (startHandshake != null)
                {
                    //First goal is the handshake at this point in the algorithm.
                    goalHandshake = new Node(currentNode.state, Vector2Int.zero, cost, nodeIndex);
                    end = true;
                    //Console.WriteLine("Found handshake: " + currentNode.state);
                    foundGoal = true;
                    return;
                }
                else if (cell == CellState.Free || cell == CellState.Robot || cell == CellState.Goal)
                {
                    foreach (Vector2Int delta in deltas)
                    {
                        Node newNode = new Node(currentNode.state + delta, delta, cost, nodeIndex);
                        if (usedNodes.Find(X => X.state == newNode.state) == null && goalFrontier.Find(X => X.state == newNode.state) == null)
                            goalFrontier.Add(newNode);
                    }
                }
            }
        }
    }

    protected override void CreateUpdatePath()
    {
        if (foundGoal)
        {
            Node nextNode = startHandshake;
            while (nextNode.parentIndex != -1)
            {
                path.Insert(0, usedNodes[nextNode.parentIndex]);
                nextNode = usedNodes[nextNode.parentIndex];
            }
            //path.Add(startHandshake);
            nextNode = goalHandshake;
            while (nextNode.parentIndex != -1)
            {
                path.Add(usedNodes[nextNode.parentIndex]);
                nextNode = usedNodes[nextNode.parentIndex];
            }
            firstGoal = path[path.Count - 1];
            virtualState = firstGoal.state;
            transform.position = env.CellRepPos(firstGoal.state);
        }

        GenerateLineVisual();

        BaseEndUS();
    }

    protected override void LogMemory()
    {
        BuildDebug.Log("End Memory Used: " + usedNodes.Count + " Nodes in complete tree + "
            + path.Count + " Nodes in main Branch = " + (path.Count + usedNodes.Count));
    }
}
