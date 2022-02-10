using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineUpdatable : DrawLine
{
    // Update is called once per frame
    void Update()
    {
        //Update the line-connected points
        _line.SetPosition(0, transform.position);
        _line.SetPosition(1, To.position);
    }
}
