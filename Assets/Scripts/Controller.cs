using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Environment env;
    public Robot robot;
    public List<Robot> robots = new List<Robot>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Robot R in robots)
            R.Init(env);
    }

    void Update()
    {
        int i = 0;
        foreach (Robot R in robots) {
            ++i;
            R.UpdateSolutionControl(CheckKey(i));
        }
    }

    private bool CheckKey(int value)
    {
        if (value == 1)
            return Input.GetKey(KeyCode.Alpha1);
        else if (value == 2)
            return Input.GetKey(KeyCode.Alpha2);
        else if (value == 3)
            return Input.GetKey(KeyCode.Alpha3);
        else if (value == 4)
            return Input.GetKey(KeyCode.Alpha4);
        else if (value == 5)
            return Input.GetKey(KeyCode.Alpha5);
        else if (value == 6)
            return Input.GetKey(KeyCode.Alpha6);
        else if (value == 7)
            return Input.GetKey(KeyCode.Alpha7);
        else if (value == 8)
            return Input.GetKey(KeyCode.Alpha8);
        else if (value == 9)
            return Input.GetKey(KeyCode.Alpha9);
        else
            return Input.GetKey(KeyCode.Alpha0);
    }
}
