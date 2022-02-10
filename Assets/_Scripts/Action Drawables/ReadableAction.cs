using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableAction : ActionDrawable
{
    protected override void PerformAction()
    {
        //Activate the readable
        GetComponentInChildren<ReadableObjectBehaviour>().Read();
    }
}
