using System.Collections.Generic;
using UnityEngine;

public class InteractionToggleObject : MonoBehaviour
{
    //A list of GameObjects to activate
    public List<GameObject> Objects = new List<GameObject>();

    //The player global variables script (to deactivate movement)
    private PlayerGlobalVariables _playerMovement;

    //The FMOD ref (To play audio shot)
    private FMODLittleGirl _fmod;

    private void Awake()
    {
        //Instance creation
        _fmod = FindObjectOfType<FMODLittleGirl>();
        _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGlobalVariables>();
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if(other.CompareTag("Player"))
    //     {
    //         
    //         
    //         //If the player interacts
    //         if(Input.GetKeyDown(KeyCode.E))
    //         {
    //             //If the object is inactive
    //             if (!Objects[1].activeSelf)
    //             {
    //                 //Activate the object and block player movement
    //                 ToggleObject(true);
    //                 _playerMovement.CanMove = false;
    //                 //Load and play FMOD event
    //                 _fmod.PlayOneShotAudio("event:/Grab");
    //             }
    //             else
    //             {
    //                 //Deactivate the object and allow player movement
    //                 ToggleObject(false);
    //                 _playerMovement.CanMove = true;
    //             }
    //         }
    //     }
    // }

    public void ToggleObject(bool activeState)
    {
        //Activate or deactivate the objects from the list
        foreach(GameObject obj in Objects)
        {
            obj.SetActive(activeState);
            _playerMovement.CanMove = !activeState;

            if (activeState)
            {
                _fmod.PlayOneShotAudio("event:/Grab");
            }
        }
        
        if (!Objects[1].activeSelf)
        {
            //Activate the object and block player movement
            ToggleObject(true);
            //Load and play FMOD event
            _fmod.PlayOneShotAudio("event:/Grab");
        }
        else
        {
            //Deactivate the object and allow player movement
            ToggleObject(false);
            _playerMovement.CanMove = true;
        }
    }
}
