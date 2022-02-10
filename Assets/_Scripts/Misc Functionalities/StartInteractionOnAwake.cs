using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteractionOnAwake : MonoBehaviour
{
    //The interaction that will start
    public Interaction TargetInteraction;

    //Check if the mouse should be drawn
    public bool ShouldDrawMouse = true;

    //Check if the drawables should be drawn
    public bool ShouldDrawDrawables = false;

    // Start is called before the first frame update
    void Awake()
    {
        TargetInteraction.StartInteraction();

        if (ShouldDrawMouse)
        {
            PlayerGlobalVariables.instance.ShouldDrawCursor = true;
        }

        if (ShouldDrawDrawables)
        {
            PlayerGlobalVariables.instance.ShouldDraw = true;
        }
    }
}
