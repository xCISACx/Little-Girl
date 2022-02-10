using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableCanvasObjectBehaviour : MonoBehaviour
{
    public Camera
        //The camera to which the readable will exit
        ExitToCamera,
        //The camera which allows the player to read the readable
        ReadableCamera;

    //Check if the player was moving before the readable was activated
    [SerializeField]
    private bool _wasMovingBefore;

    FMODLittleGirl fmod;

    // Start is called before the first frame update
    void Awake()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
        ReadableCamera = GameObject.FindWithTag("ReadableCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //Makes sure that the player is not on on the pause menu
            if (Time.timeScale > 0f)
            {
                //Deactivate the object and allow player movement
                DisableReadable(gameObject);
            }
        }

        //Corner case: if the player tries to pause whilst reading, the readable must be deactivated
        //If the player presses escape, it doesn't matter if they are in the main menu, it will exit anyways
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Deactivate the object and allow player movement
            DisableReadable(gameObject);
        }
    }

    private void OnEnable()
    {
        //Set the exit camera as the one enabled before the readable was activated
        ExitToCamera = PlayerGlobalVariables.instance.CurrentCamera;

        //Deactivate the exit camera
        ExitToCamera.gameObject.SetActive(false);

        //Set the current camera as the readable camera
        PlayerGlobalVariables.instance.CurrentCamera = ReadableCamera;

        //Play the readable activation sound according to the readable type
        if (transform.parent.name.Contains("Newspaper"))
        {
            fmod.PlayOneShotAudio("event:/SFX/Readables/SFX_Readables_Newspaper");
        }
        else if (transform.parent.name.Contains("Book"))
        {
            fmod.PlayOneShotAudio("event:/SFX/Readables/SFX_Readables_Book");
        }
        else if (transform.parent.name.Contains("Letter"))
        {
            fmod.PlayOneShotAudio("event:/SFX/Readables/SFX_Readables_Sheet");
        }
    }

    public void DisableReadable(GameObject readable)
    {
        //Offset the target position ever-so-slightly to avoid action overlap
        PointAndClickBehaviour.instance.destinationPos += new Vector3(0.0001f, 0, 0);

        //If movement wasn't allowed, disable it again
        if (_wasMovingBefore)
        {
            PlayerGlobalVariables.instance.ToggleMovement(true);
        }
        else
        {
            PlayerGlobalVariables.instance.ToggleMovement(false);
        }

        //Deactivate the readable camera
        ReadableCamera.gameObject.SetActive(false);

        //Activate the exit camera
        ExitToCamera.gameObject.SetActive(true);

        //Set the current camera as the exit camera
        PlayerGlobalVariables.instance.CurrentCamera = ExitToCamera;

        //Deactivate the readable
        readable.gameObject.SetActive(false);
    }

    private IEnumerator WaitBeforeTogglingMovement(bool movementState)
    {
        yield return new WaitForSecondsRealtime(0.01f);

        PlayerGlobalVariables.instance.ToggleMovement(movementState);
    }

    public void CouldMoveBefore(bool couldMove)
    {
        //Set the "could move before" bool
        _wasMovingBefore = couldMove;
    }
}
