using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjectManipulation : MonoBehaviour
{
    //The object that will be toggled
    public GameObject ToToggle;

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    protected void ToggleObject()
    {
        ToToggle.SetActive(!ToToggle.activeSelf);
    }

    protected void SetObject(bool newActiveState)
    {
        ToToggle.SetActive(newActiveState);
    }

    protected IEnumerator WaitSetObject(bool newActiveState)
    {
        yield return new WaitForEndOfFrame();

        SetObject(newActiveState);
    }
}
