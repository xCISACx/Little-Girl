using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrawables : MonoBehaviour
{
    //The actions available for this camera
    public ActionDrawable[] ActionDrawablesForCamera;

    private void OnEnable()
    {
        //If there are drawables to enable, enable them
        if (ActionDrawablesForCamera.Length != 0)
        {
            ToggleActionDrawables(true);
        }
        else
        {
            //If there isn't any drawable to draw, disable all drawables
            foreach(ActionDrawable actionDrawable in PlayerGlobalVariables.instance.AllActionDrawables)
            {
                if (actionDrawable != null)
                {
                    actionDrawable.enabled = false;
                }
            }
        }
    }

    private void OnDisable()
    {
        //If there are drawables to disable, disable them
        if (ActionDrawablesForCamera.Length != 0)
        {
            ToggleActionDrawables(false);
        }
        else
        {
            //If there are no drawables to disable, enable the current camera's drawables
            PlayerGlobalVariables.instance.CurrentCamera.GetComponent<CameraDrawables>().ToggleActionDrawables(true);
        }
    }

    private void ToggleActionDrawables(bool activeState)
    {
        //Toggle all action drawables for the camera
        foreach (ActionDrawable action in ActionDrawablesForCamera)
        {
            if (action != null)
            {
                action.enabled = activeState;
            }
        }
    }
}
