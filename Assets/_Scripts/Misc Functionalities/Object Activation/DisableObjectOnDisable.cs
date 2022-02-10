using System.Collections;
using UnityEngine;

public class DisableObjectOnDisable : BaseObjectManipulation
{
    protected override void OnDisable()
    {
        SetObject(false);
    }
}
