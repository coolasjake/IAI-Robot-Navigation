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

    public static string GetDirectory()
    {
        if (File.Exists("FileToLoad.txt"))
        {
            BuildDebug.Log("Directory instructions found");
            StreamReader SR = new StreamReader("FileToLoad.txt");
            return SR.ReadLine();
        }
        else
            BuildDebug.Log("Directory instructions missing!");
        return "Environments/RobotNav-test.txt";
    }

    public static RawEnvironment LoadEnvironment(string path)
    {
        if (!File.Exists(path))
        {
            BuildDebug.Log("ERROR loading chosen directory. Swapping to default Environment");
            if (File.Exists("Environments/RobotNav-test.txt"))
                path = "Environments/RobotNav-test.txt";
            else
            {
                BuildDebug.Log("ERROR loading default directory.");
                path = "";
            }
        }

        if (File.Exists(path))
        {
            BuildDebug.Log("Loading Chosen Environment");
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
            //Swap the x and y values to make later code simpler.
            int temp = RE.gridSize.x;
            RE.gridSize.x = RE.gridSize.y;
            RE.gridSize.y = temp;
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
