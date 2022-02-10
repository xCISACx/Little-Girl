using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class ObservationDrawable : ActionDrawable
{
    //The observation intertitle parent
    public GameObject ObservationIntertitle;
    //The observation intertitle's behaviour script
    protected ObservationIntertitleBehaviour _IntertitleBehaviour;

    //The observation intertitle's text
    public TextMeshProUGUI ObservationIntertitleText;

    //The time this observation is active
    public float ObservationIntertitleTime = 3f;

    //This observation's text
    public string ObservationText;

    //Check if this observation can be skipped
    public bool CanSkip = true;

    #region Player variables
    //The player's navigation agent and point-and-click manager
    private NavMeshAgent _playerNavMeshAgent;
    private PointAndClickBehaviour _playerPointAndClick;

    //The player's navigation agent's speed and acceleration
    private float
        _navMeshAgentSpeed,
        _navMeshAgentAcceleration;
    #endregion

    protected void Start()
    {
        ObservationIntertitle = GameObject.FindGameObjectWithTag("ObservationIntertitle").transform.GetChild(0).gameObject;
        _IntertitleBehaviour = ObservationIntertitle.GetComponentInChildren<ObservationIntertitleBehaviour>(true);
        ObservationIntertitleText = ObservationIntertitle.GetComponentInChildren<TextMeshProUGUI>(true);

        _playerNavMeshAgent = PlayerGlobalVariables.instance.GetComponent<NavMeshAgent>();
        _navMeshAgentSpeed = _playerNavMeshAgent.speed;
        _navMeshAgentAcceleration = _playerNavMeshAgent.acceleration;
        _playerPointAndClick = PlayerGlobalVariables.instance.GetComponent<PointAndClickBehaviour>();
    }

    protected override void Update()
    {
        DefineButtonSize();

        if (PlayerGlobalVariables.instance.ShouldDraw)
        {
            _DistanceToMouse = Vector2.Distance(new Vector2(                                                                                        //Adjustment to the button's size
                                                            PlayerGlobalVariables.instance.CurrentCamera.WorldToScreenPoint(transform.position).x,  //- _ActualSize.x / 2,
                                                            PlayerGlobalVariables.instance.CurrentCamera.WorldToScreenPoint(transform.position).y   //- _ActualSize.y / 2
                                                           ),
                                                Input.mousePosition
                                               );

            //Calculate the target opacity depending on the mouse position versus the "show" hitbox
            _Opacity = Mathf.Clamp01(1 - (_DistanceToMouse / (_DistanceNeededToShow.x / 2)));

            #region New way of recognizing mouse position vs button position
            //If the mouse is outside the circle that shows the button, hide it
            if (_DistanceToMouse > _DistanceNeededToShow.x / 2f)
            {
                //Change the button's texture and size to "normal"
                //ButtonTexture = null;
                //_Opacity = 0f;

                //Reset the current interactable if it's this one
                ResetCurrentInteractableIfEqual();
            }
            //If the mouse is inside the circle that shows the button and outside the button
            else if (_DistanceToMouse <= _DistanceNeededToShow.x / 2f && _DistanceToMouse > _ActualSize.x / 2f)
            {
                //Change the button's texture and size to "normal"
                ButtonTexture = NormalButtonTexture;
                _ActualSize = _NormalSize;

                //Reset the current interactable if it's this one
                ResetCurrentInteractableIfEqual();
            }
            //If the mouse is inside the button
            else if (_DistanceToMouse <= _ActualSize.x / 2f)
            {
                //Change the button's texture and size to "hover"
                ButtonTexture = HoverButtonTexture;
                _ActualSize = _HoverSize;

                //Set the current interactable as this object (because it is hovered)
                GameManager.instance.currentInteractable = gameObject;

                //If the player clicks
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.Log("Should perform action");
                    //Perform the action
                    PerformAction();
                }
            }
            #endregion
        }
    }

    protected override void GoToAction()
    {

    }

    protected override void PerformAction()
    {
        //Hide drawables and deactivate player movement
        _IntertitleBehaviour.SetCouldMoveBefore(PlayerGlobalVariables.instance.CanMove);
        _IntertitleBehaviour.TogglePlayerControl(false);
        PlayerGlobalVariables.instance.TemporarilyDeactivatePlayerMovement();

        //Set the intertitle text as the one desired for this interaction
        //ObservationIntertitleText.text = ObservationText;
        ObservationIntertitleText.text = ObservationText.Replace("lnbrk", "\n");

        //Activate the intertitle
        ObservationIntertitle.SetActive(true);

        //Start the coroutine that hides the intertitle if the waiting time isn't 0
        if (ObservationIntertitleTime != 0)
        {
            _IntertitleBehaviour.LastCoroutine = StartCoroutine(_IntertitleBehaviour.HideObservation(ObservationIntertitleTime, CanSkip));
        }
    }

    protected bool CheckObservationActiveState()
    {
        //Returns whether the observation intertitle is active
        return _IntertitleBehaviour.gameObject.activeSelf;
    }
}
