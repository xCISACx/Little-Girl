using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class OneTimeOnlyObservationDrawable : ObservationDrawable
{
    public string FMODSoundEvent;

    public bool MusicBoxHide = true;

    protected override void PerformAction()
    {
        //Hide drawables and deactivate player movement
        _IntertitleBehaviour.TogglePlayerControl(false);
        PlayerGlobalVariables.instance.TemporarilyDeactivatePlayerMovement();

        //Set the intertitle text as the one desired for this interaction
        //ObservationIntertitleText.text = ObservationText;
        ObservationIntertitleText.text = ObservationText.Replace("lnbrk", "\n");

        //Activate the intertitle
        ObservationIntertitle.SetActive(true);

        //Start the coroutine that hides the intertitle
        _IntertitleBehaviour.LastCoroutine = StartCoroutine(_IntertitleBehaviour.HideObservation(ObservationIntertitleTime, CanSkip));

        //TODO: Add the FMOD event name here if needed to call it

        //If it should, get the music box's script and mark this drawable as read
        if (MusicBoxHide)
        {
            GetComponentInParent<MusicBoxPart2Manager>().MarkAsSeen(this);
        }

        //Disable this observation's interaction icon
        Destroy(this);
    }
}
