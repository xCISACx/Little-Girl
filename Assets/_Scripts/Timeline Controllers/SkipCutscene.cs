using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkipCutscene : MonoBehaviour
{
    private PlayableDirector _currentTimeline;

    private void OnEnable()
    {
        _currentTimeline = transform.parent.parent.parent.GetComponent<PlayableDirector>();
        StartCoroutine(DisableSkipCutsceneNotice());
    }

    // Update is called once per frame
    void Update()
    {
        //If the user presses the right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            //Makes sure the game isn't paused
            if (Time.timeScale > 0f)
            {
                //Makes sure the cutscene is running (to prevent skipping in the middle of an interaction)
                if (_currentTimeline.enabled)
                {
                    //Start the next cutscene
                    transform.parent.parent.parent.GetComponent<PlayTimelineOnClipEnding>().StartOtherTimeline();

                    transform.parent.parent.parent.GetComponent<PlayTimelineOnClipEnding>().enabled = false;
                }
            }
        }
    }

    private IEnumerator DisableSkipCutsceneNotice()
    {
        yield return new WaitForSecondsRealtime(3f);

        //Deactivate the notice
        gameObject.SetActive(false);
    }
}
