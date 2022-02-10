using System.Collections;
using UnityEngine;

public class DisableObjectOnEnable : BaseObjectManipulation
{
    protected override void OnEnable()
    {
        StartCoroutine(WaitSetObject(false));
    }
}
