using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCell : MonoBehaviour
{
    public TextMesh text3D;

    public string Coords
    {
        set { text3D.text = value; }
    }

    public Color color
    {
        set { text3D.color = value; }
        get { return text3D.color; }
    }
}
