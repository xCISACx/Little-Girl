using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartInteractionDisableTimeline : StartInteractionOnAwake
{
    // Start is called before the first frame update
    void Awake()
    {
        //transform.GetComponentInParent<PlayableDirector>().enabled = false;

        TargetInteraction.StartInteraction();
        PlayerGlobalVariables.instance.CurrentCamera = TargetInteraction.InteractionCamera;

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
