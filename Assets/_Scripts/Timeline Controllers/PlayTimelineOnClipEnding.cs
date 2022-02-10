using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayTimelineOnClipEnding : MonoBehaviour
{
    //This timeline
    protected PlayableDirector _ThisTimeline;

    //The timeline that will start just as this one ends
    public PlayableDirector OtherTimeline;

    //Check if, after this cutscene, the interaction points should be drawn
    public bool ShouldDrawAfterCutscene;

    //Check if, after this cutscene, the player's movement should be enabled
    public bool ShouldMoveAfterCutscene;

    // Start is called before the first frame update
    protected IEnumerator Start()
    {
        //Instance creation
        _ThisTimeline = GetComponent<PlayableDirector>();

        yield return new WaitForEndOfFrame();

        //Reset the initial time (after a frame so that it does not automatically destroy)
        _ThisTimeline.initialTime = 0f;
    }

    // Update is called once per frame
    protected void Update()
    {
        //If the timeline's time is 0 (end was reached), start the next timeline
        if(_ThisTimeline.time == 0)
        {
            StartOtherTimeline();
            enabled = false;
        }

        //If the user presses any button
        if (Input.anyKeyDown)
        {
            //Makes sure the game isn't paused
            if (Time.timeScale > 0f)
            {
                //Makes sure the cutscene is running to avoid skipping during an interaction
                if (_ThisTimeline.enabled)
                {
                    //Activate the notice to skip the cutscene
                    transform.GetChild(0).GetComponentInChildren<SkipCutscene>(true).gameObject.SetActive(true);
                }
            }
        }
    }

    protected void LateUpdate()
    {
        //Make sure the player cannot move during the cutscene
        PlayerGlobalVariables.instance.CanMove = false;
    }

    public virtual void StartOtherTimeline()
    {
        _ThisTimeline.Stop();

        //Activate and play the next timeline
        OtherTimeline.gameObject.SetActive(true);
        OtherTimeline.Play();

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
