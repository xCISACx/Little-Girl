using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteractionOnDestroy : MonoBehaviour
{
    //The interaction that will start
    public Interaction TargetInteraction;

    //Check if the mouse should be drawn
    public bool ShouldDrawMouse = true;

    // Start is called before the first frame update
    void OnDestroy()
    {
        TargetInteraction.StartInteraction();

        if(ShouldDrawMouse)
        {
            PlayerGlobalVariables.instance.ShouldDrawCursor = true;
        }
    }
}
