using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MusicBoxPart2Manager : MonoBehaviour
{
    //A list of drawables which should enable when the box is open
    public List<ActionDrawable> DrawablesToActivate = new List<ActionDrawable>();

    //The last drawable (this drawable will only appear when all others were seen and will trigger the next cutscene)
    public ActionDrawable LastDrawable;

    //The interaction script
    private Interaction _interaction;

    //The timeline that will play when this drawable is selected
    public PlayableDirector TimelineToActivate;

    // Start is called before the first frame update
    void Start()
    {
        _interaction = GetComponent<Interaction>();

        _interaction.Stopped.AddListener(OnInteractionStopped);

        //Start the second interaction
        _interaction.StartInteraction();

        //Deactivate player movement (just to be sure)
        PlayerGlobalVariables.instance.CanMove = false;

        //Start the coroutine that activates the drawables
        StartCoroutine(ActivateObservationDrawables());
    }

    public void OnInteractionStopped()
    {
        //Play the timeline
        TimelineToActivate.gameObject.SetActive(true);
        TimelineToActivate.Play();

        //Deactivate the interaction's camera
        _interaction.InteractionCamera.gameObject.SetActive(false);
        //Deactivate the interaction's music box
        gameObject.SetActive(false);
    }

    private IEnumerator ActivateObservationDrawables()
    {
        yield return new WaitForSecondsRealtime(5f);

        //Activates all drawables that should be activated in the beginning of the interaction
        foreach(ActionDrawable drawable in DrawablesToActivate)
        {
            drawable.gameObject.SetActive(true);
        }
    }

    public void MarkAsSeen(ActionDrawable drawable)
    {
        //If the drawables-to-activate list has the target drawable
        if (DrawablesToActivate.Contains(drawable))
        {
            //Mark it as read by removing it from the list and destroying its drawable
            DrawablesToActivate.Remove(drawable);
            Destroy(drawable);
        }

        //If the list is empty (if the player has seen all drawables)
        if(DrawablesToActivate.Count == 0)
        {
            //Enable the last drawable
            LastDrawable.gameObject.SetActive(true);
        }
    }
}
