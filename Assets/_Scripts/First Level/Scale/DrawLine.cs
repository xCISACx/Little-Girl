using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    //The transform to which the line will be drawn towards
    public Transform To;

    //The line renderer
    protected LineRenderer _line;

    //The line's material
    public Material LineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //Add the line component
        _line = gameObject.AddComponent<LineRenderer>();

        //Set the line's material
        _line.material = LineMaterial;

        //Set the line's width
        _line.startWidth = 0.01f;
        _line.endWidth = 0.01f;

        //Set the line's colour
        _line.startColor = Color.black;
        _line.endColor = Color.black;

        //Set the positions which the line will connect
        _line.positionCount = 2;
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, To.position);
    }
}
