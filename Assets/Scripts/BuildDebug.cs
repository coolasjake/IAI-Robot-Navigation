using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class BuildDebug : MonoBehaviour
{
    private static Text Output;

    void Awake()
    {
        Output = GetComponent<Text>();
    }

    public static void Log(string text)
    {
        Output.text += text + "\n";
    }

    public static void Log(object message)
    {
        Log(message.ToString());
    }
}
