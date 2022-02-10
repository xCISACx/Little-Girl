using System.Collections;
using UnityEngine;

public class EnableObjectOnDisable : BaseObjectManipulation
{
    protected override void OnDisable()
    {
        SetObject(true);
    }
}
