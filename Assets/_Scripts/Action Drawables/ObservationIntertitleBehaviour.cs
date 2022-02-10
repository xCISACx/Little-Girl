using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationIntertitleBehaviour : MonoBehaviour
{
    //Used to store the last coroutine started (to avoid overlapping)
    public Coroutine LastCoroutine;

    //Check if the user can skip this observation
    private bool _canHide = true;

    //Check if the user could move before this intertitle
    private bool _couldMoveBefore;

    //The pause menu (for reactivating drawables if the player pauses mid-intertitle)
    private PauseMenuActivation _pauseMenu;

    private void Awake()
    {
        _pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<PauseMenuActivation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_canHide)
            {
                if (Time.timeScale > 0f)
                {
                    //Hide the intertitle
                    DeactivateIntertitle();
                }
            }
        }

        //Corner case: if the player tries to pause whilst an observation intertitle is active, the intertitle must be deactivated
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_canHide)
            {
                //Hide the intertitle
                gameObject.SetActive(false);

                PlayerGlobalVariables.instance.ReactivatePlayerMovement();

                _pauseMenu.ShouldDrawOnContinue = true;
                _pauseMenu.ShouldDrawCursorOnContinue = true;
            }
        }
    }

    public IEnumerator HideObservation(float timeUntilDeactivation, bool canHide)
    {
        yield return new WaitForSecondsRealtime(timeUntilDeactivation);
        
        DeactivateIntertitle();
    }

    public void DeactivateIntertitle()
    {
        //Reactivate drawables and player movement
        TogglePlayerControl(true);
        PlayerGlobalVariables.instance.ReactivatePlayerMovement();

        //Hide the intertitle
        gameObject.SetActive(false);
    }

    public void TogglePlayerControl(bool toggleState)
    {
        //If the toggleState is true, we need to check if the player could move before the intertitle was activated
        if (toggleState == true)
        {
            if (_couldMoveBefore == true)
            {
                PlayerGlobalVariables.instance.CanMove = toggleState;
            }
        }
        else
        {
            PlayerGlobalVariables.instance.CanMove = toggleState;
        }

        //Set the should-draw bools as the toogleState
        PlayerGlobalVariables.instance.ShouldDraw = toggleState;
        PlayerGlobalVariables.instance.ShouldDrawCursor = toggleState;
    }

    public void SetCouldMoveBefore(bool couldMove)
    {
        //Set the "could move before" bool
        _couldMoveBefore = couldMove;
    }

    private void OnDisable()
    {
        //Make sure there is no deactivation timer overlap
        StopCoroutine(LastCoroutine);
    }
}
