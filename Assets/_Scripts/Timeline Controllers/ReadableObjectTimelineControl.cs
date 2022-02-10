using UnityEngine;
using UnityEngine.Playables;

public class ReadableObjectTimelineControl : ReadableObjectBehaviour
{
    //The target timeline's controller
    public PlayableDirector TimelineController;

    //Check if the readable should be added to the inventory when it's disabled
    public bool ShouldAddToInventory = true;

    private void Start()
    {
        //Disable the timeline so that it stops
        TimelineController.enabled = false;
    }

    private void Update()
    {
        //Check for a right mouse button press
        if(Input.GetMouseButtonDown(1))
        {
            //Make sure the game isn't paused
            if (Time.timeScale > 0f)
            {
                //Add the readable to the inventory
                AddToInventory();

                //Resume the timeline
                TimelineController.enabled = true;

                //Deactivate this object
                gameObject.SetActive(false);
            }
        }
    }

    private void AddToInventory()
    {
        var readable = GetComponent<ReadableProperties>();

        if (!GameManager.instance.CollectedReadables.Contains(readable))
        {
            Debug.Log("readable not in inventory yet, adding " + readable.gameObject.name + " to the inventory");

            GameManager.instance.CollectedReadables.Add(readable);

            switch (readable.Type)
            {
                case ReadableProperties.ReadableType.Letter:
                    GameManager.instance.CollectedLetters.Add(readable);
                    GameManager.instance.CreateContainer(GameManager.instance.LetterPanel, readable);
                    break;

                case ReadableProperties.ReadableType.Book:
                    GameManager.instance.CollectedBooks.Add(readable);
                    GameManager.instance.CreateContainer(GameManager.instance.BookPanel, readable);
                    break;
                case ReadableProperties.ReadableType.Newspaper:
                    GameManager.instance.CollectedNewspapers.Add(readable);
                    GameManager.instance.CreateContainer(GameManager.instance.NewspaperPanel, readable);
                    break;
            }
        }
    }
}
