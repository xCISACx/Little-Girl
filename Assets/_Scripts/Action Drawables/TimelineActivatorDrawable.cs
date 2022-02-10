using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.Playables;

public class TimelineActivatorDrawable : OneTimeOnlyObservationDrawable
{
    protected override void PerformAction()
    {
        //TODO: Add the FMOD event name here if needed to call it

        //If it should, get the parent object's interaction script and exit the interaction
        if (MusicBoxHide)
        {
            //Exit the interaction
            transform.parent.GetComponent<Interaction>().ExitInteraction();
        }

        //Hide the cursor and the drawables
        PlayerGlobalVariables.instance.ShouldDraw = false;
        PlayerGlobalVariables.instance.ShouldDrawCursor = false;
        
        //Disable this observation's interaction icon
        Destroy(this);
    }
}
