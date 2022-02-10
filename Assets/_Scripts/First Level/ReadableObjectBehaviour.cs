using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class ReadableObjectBehaviour : MonoBehaviour
{
    private FMODLittleGirl fmod;

    //The interaction pointing at this readable
    public Interaction Interaction;

    public Camera
        //The camera enabled before the readable was activated
        ExitToCamera,
        //The camera that allows the player to read the readable
        ReadableCamera;

    //The readable's image
    public Image Book;

    //The icons that inform the player that they have read this readable
    public Texture2D
        ReadButtonTexture,
        ReadHoverButtonTexture;
    
    public void OnInteractionStarted()
    {
        enabled = true;
    }
    
    public void OnInteractionStopped()
    {
        enabled = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();

        enabled = false;
    }

    public void ToggleObject(bool activeState)
    {
        if(activeState == true)
        {
            //Set the "movement-allowed-before" bool
            Book.GetComponent<ReadableCanvasObjectBehaviour>().CouldMoveBefore(PlayerGlobalVariables.instance.CanMove);
        }

        //Toggle player movement according to the readable's active state
        PlayerGlobalVariables.instance.CanMove = !activeState;
        
        //Get the readable's properties
        var readable = transform.parent.GetComponent<ReadableProperties>();
        
        //Collect this readable
        Collect(readable);

        //Activate or deactivate the objects from the list
        ReadableCamera.gameObject.SetActive(activeState);
        Book.gameObject.SetActive(activeState);
    }

    public void Read()
    {
        //If the object is inactive
        if (!Book.gameObject.activeSelf)
        {
            //Activate the object and block player movement
            ToggleObject(true);

            //Mark the readable as collected
            MarkAsRead();

            Book.GetComponent<ReadableCanvasObjectBehaviour>();
        }
    }

    public void Collect(ReadableProperties readable)
    {
        if (!GameManager.instance.CollectedReadables.Contains(readable))
        {
            //Add the readable to the global collected readables
            GameManager.instance.CollectedReadables.Add(readable);

            //Add the readable to the type-specific collected readables
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

            //Check if there is a readable action (there may not be in some automatically-activated readables
            if (transform.parent.GetComponent<ReadableAction>() != null)
            {
                //Check if the button's texture is different to the read texture
                if (transform.parent.GetComponent<ReadableAction>().ButtonTexture != ReadButtonTexture)
                {
                    //Set the button's texture as "read"
                    MarkAsRead();
                }
            }
        }
    }
    
    public void MarkAsRead()
    {
        //Set all of the readable's icons as "collected readable" icons
        ReadableAction readable = transform.parent.GetComponent<ReadableAction>();
        if (readable != null)
        {
            readable.ButtonTexture = ReadButtonTexture;
            readable.NormalButtonTexture = ReadButtonTexture;
            readable.HoverButtonTexture = ReadHoverButtonTexture;
        }
    }
}
