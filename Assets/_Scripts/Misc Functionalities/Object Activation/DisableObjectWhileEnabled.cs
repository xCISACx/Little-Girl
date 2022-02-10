using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectWhileEnabled : BaseObjectManipulation
{
    private bool _wasActiveBefore;

    protected override void OnEnable()
    {
        _wasActiveBefore = ToToggle.activeSelf;

        SetObject(false);
    }

    protected override void OnDisable()
    {
        if(_wasActiveBefore)
        {
            SetObject(true);
        }
    }
}
