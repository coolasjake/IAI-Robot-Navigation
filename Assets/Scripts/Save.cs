using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Save : MonoBehaviour
{

    /// <summary> List of directories for the units in the players collection. </summary>
    public static List<string> Collection = new List<string>();
    public static string CurrentNodeName;
    public static int PlayerLevel = 3;
    List<string> lines = new List<string>();

    public static RawEnvironment LoadEnvironment(string path)
    {
        if (File.Exists(path))
        {
            RawEnvironment RE = new RawEnvironment();
            List<string> lines = new List<string>();
            StreamReader SR = new StreamReader(path);
            while (!SR.EndOfStream)
                lines.Add(SR.ReadLine());

            if (lines.Count < 3)
            {
                Debug.LogError("Environment file incomplete");
                return new RawEnvironment();
            }
            
            RE.gridSize = lines[0].StringToV2();
            RE.startPos = lines[1].StringToV2();
            string[] goals = lines[2].Split('|');
            RE.goalsPos = new List<Vector2Int>();
            foreach (string G in goals)
                RE.goalsPos.Add(G.StringToV2());
            lines.RemoveRange(0, 3);

            if (lines.Count == 0)
            {
                Debug.LogWarning("No walls in Environment");
                return new RawEnvironment();
            }

            RE.wallRects = new List<Rect>();
            foreach (string wall in lines)
                RE.wallRects.Add(wall.StringToRect());

            return RE;
        }
        return new RawEnvironment();
    }
}

[Serializable]
public class RawEnvironment
{
    public Vector2Int gridSize = Vector2Int.zero;
    public Vector2Int startPos = Vector2Int.zero;
    public List<Vector2Int> goalsPos = new List<Vector2Int>();
    public List<Rect> wallRects = new List<Rect>();
}
