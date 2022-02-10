using System;
using System.Collections;
using UnityEngine;

public class ToggleObjectOnEnable : BaseObjectManipulation
{
    protected override void OnEnable()
    {
        ToggleObject();
    }

    protected override void OnDisable()
    {
        ToggleObject();
    }
}
