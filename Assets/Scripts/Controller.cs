using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Environment env;
    //public Robot robot;
    public List<Robot> robots = new List<Robot>();
    public Camera cam;
    public Camera textCam;
    private float cameraZoom = 5;
    public float cameraDefault = 5;
    public float maxCameraSize = 15;
    public float minCameraSize = 3;
    public float scrollSense = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        foreach (Robot R in robots)
            R.Init(env);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BuildDebug.Log("Ending Program");
            Application.Quit();
        }

        if (Input.mouseScrollDelta != Vector2.zero)
        {
            cameraZoom = Mathf.Clamp(cameraZoom - (Input.mouseScrollDelta.y * scrollSense), minCameraSize, maxCameraSize);
            cam.orthographicSize = cameraZoom;
            textCam.orthographicSize = cameraZoom;
        }

        if (Input.GetMouseButtonDown(2))
        {
            cameraZoom = Mathf.Clamp(cameraDefault, minCameraSize, maxCameraSize);
            cam.orthographicSize = cameraZoom;
            textCam.orthographicSize = cameraZoom;
        }

        int i = 0;
        foreach (Robot R in robots) {
            ++i;
            R.UpdateSolutionControl(CheckKey(i));
        }
    }

    private bool CheckKey(int value)
    {
        if (value == 1)
            return Input.GetKeyDown(KeyCode.Alpha1);
        else if (value == 2)
            return Input.GetKeyDown(KeyCode.Alpha2);
        else if (value == 3)
            return Input.GetKeyDown(KeyCode.Alpha3);
        else if (value == 4)
            return Input.GetKeyDown(KeyCode.Alpha4);
        else if (value == 5)
            return Input.GetKeyDown(KeyCode.Alpha5);
        else if (value == 6)
            return Input.GetKeyDown(KeyCode.Alpha6);
        else if (value == 7)
            return Input.GetKeyDown(KeyCode.Alpha7);
        else if (value == 8)
            return Input.GetKeyDown(KeyCode.Alpha8);
        else if (value == 9)
            return Input.GetKeyDown(KeyCode.Alpha9);
        else
            return Input.GetKeyDown(KeyCode.Alpha0);
    }
}
