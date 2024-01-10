using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    //Check if the player can interact
    public bool CanInteract;

    //Check if the player is interacting
    public bool IsInteracting;

    public bool
        //Check if the player can exit this interaction *at the moment*
        CanExitInteraction = true,
        //Check if the player can exit this interaction *at any time*
        CanExitInteractionGeneral = true;

    //Check if this interaction requires changing the camera
    public bool RequireCameraChange;

    //The camera enabled before the interaction
    public Camera mainCamera;

    //The interaction camera
    public Camera InteractionCamera;

    //Interaction start and stop events
    public UnityEvent Started, Stopped;
    
    // Update is called once per frame
    void Update()
    {
        if (PlayerGlobalVariables.instance.CurrentCamera != InteractionCamera)
        {
            mainCamera = PlayerGlobalVariables.instance.CurrentCamera;
        }

        //If the inventory is open while interacting with a puzzle...
        if (GameManager.instance.IsInventoryExpanded)
        {
            //Don't allow the player to leave the interaction
            CanExitInteraction = false;
        }
        else
        {
            //Allow the player to leave the interaction if the inventory isn't open
            CanExitInteraction = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (CanExitInteractionGeneral)
            {
                //Make sure the game isn't paused
                if (Time.timeScale > 0f)
                {
                    if (IsInteracting)
                    {
                        if (CanExitInteraction)
                        {
                            PointAndClickBehaviour.instance.destinationPos = PointAndClickBehaviour.instance.transform.position + new Vector3(0.0001f, 0, 0);
                            ExitInteraction();
                        }
                    }
                }
            }
        }

        //Corner case: if the player tries to pause whilst interacting with a puzzle, the puzzle must be deactivated
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CanExitInteractionGeneral)
            {
                if (IsInteracting)
                {
                    if (CanExitInteraction)
                    {
                        PointAndClickBehaviour.instance.destinationPos = PointAndClickBehaviour.instance.transform.position + new Vector3(0.0001f, 0, 0);
                        ExitInteraction();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the colliding entity is the player
        if (other.gameObject.CompareTag("Player"))
        {
            //Allow the player to interact
            CanInteract = true;
        }
    }

    public void StartInteraction()
    {
        //If the interaction requires a camera change
        if (RequireCameraChange)
        {
            //Switch from the currently enabled camera to the interaction camera
            SwitchToInteractionCamera();   
        }

        //Disable the interaction-enabling collider
        transform.GetComponent<BoxCollider>().enabled = false;

        //Set the puzzle as "being interacted with"
        IsInteracting = true;

        //Disable player movement
        PlayerGlobalVariables.instance.CanMove = false;

        //Invoke the "start" events
        Started.Invoke();
    }
    
    public void ExitInteraction()
    {
        //Set the puzzle as "not being interacted with"
        IsInteracting = false;
        
        //Switch back to the camera enabled before the interaction started
        SwitchToPlayerCamera();

        //Reactivate the interaction-enabling collider
        transform.GetComponent<BoxCollider>().enabled = true;

        //Reactivate player movement
        PlayerGlobalVariables.instance.CanMove = true;

        //Invoke the "stop" events
        Stopped.Invoke();
    }
    
    private void OnTriggerExit(Collider other)
    {
        //If the player exits the colliding area
        if (other.gameObject.CompareTag("Player"))
        {
            //The player can no longer interact
            CanInteract = false;
            ExitInteraction();
        }
    }

    public void SwitchToInteractionCamera()
    {
        //Disable the current camera
        mainCamera.gameObject.SetActive(false);

        //Enable the interaction camera
        InteractionCamera.gameObject.SetActive(true);

        //Set the current camera as the interaction camera
        PlayerGlobalVariables.instance.CurrentCamera = InteractionCamera;
    }

    public void SwitchToPlayerCamera()
    {
        //Enable the previous camera
        mainCamera.gameObject.SetActive(true);

        //Set the current camera as the previous camera
        PlayerGlobalVariables.instance.CurrentCamera = mainCamera;

        //Disable the interaction camera
        InteractionCamera.gameObject.SetActive(false);
    }
}
