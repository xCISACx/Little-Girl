using System.Collections;
using UnityEngine;

public class EnableObjectOnEnable : BaseObjectManipulation
{
    protected override void OnEnable()
    {
        StartCoroutine(WaitSetObject(true));
    }
}
