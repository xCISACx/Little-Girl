using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectOnTimelineEnding : PlayTimelineOnClipEnding
{
    public GameObject ToActivate;

    public override void StartOtherTimeline()
    {
        _ThisTimeline.Stop();

        //Activate and play the next timeline
        ToActivate.SetActive(true);

        //Check if drawables should appear after this cutscene is over
        if (ShouldDrawAfterCutscene)
        {
            PlayerGlobalVariables.instance.ShouldDraw = true;
            PlayerGlobalVariables.instance.ShouldDrawCursor = true;
        }

        //Check if the player can move after this cutscene is over
        if (ShouldMoveAfterCutscene)
        {
            PlayerGlobalVariables.instance.CanMove = true;
        }

        //Destroy this timeline
        Destroy(gameObject);
    }
}
