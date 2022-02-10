using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ActionDrawable : MonoBehaviour
{
    //Indicates the position to which the player character will move to perform this action
    //Leave at (0, 0, 0) if no movement is needed
    public Transform TargetActionPosition;

    public Texture2D
        //The real-time defined texture
        ButtonTexture,
        //The button's normal texture
        NormalButtonTexture,
        //The button's hover texture
        HoverButtonTexture;

    //The player's global variables
    protected PlayerGlobalVariables _PlayerVariables;

    //The player's point and click behaviour
    protected PointAndClickBehaviour _PointAndClickBehaviour;

    protected Vector2
        //The button's real-time size
        _ActualSize,
        //The button's normal size
        _NormalSize,
        //The button's size whilst hovering a button
        _HoverSize,
        //The button's visibility toggle hitbox
        _DistanceNeededToShow;
    
    public float
        //The modifier for the icon size
        SizeModifier = 1f,
        //The modifier for the icon to appear
        ShowCircleSizeModifier = 1f;

    //The distance between the drawable and the mouse
    protected float _DistanceToMouse;

    //The value of opacity at which the button should be drawn
    protected float _Opacity = 0f;

    // Start is called before the first frame update
    protected void Awake()
    {
        //Instance creation
        _PlayerVariables = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerGlobalVariables>();
        _PointAndClickBehaviour = _PlayerVariables.GetComponent<PointAndClickBehaviour>();

        DefineButtonSize();
        _ActualSize = _NormalSize;
    }

    public void DefineButtonSize()
    {
        //Button size definition
        _NormalSize = new Vector2(Screen.height / 15f, Screen.height / 15f) * SizeModifier;
        _HoverSize = new Vector2(Screen.height / 10f, Screen.height / 10f) * SizeModifier;
        _DistanceNeededToShow = new Vector2(Screen.height / 2.5f, Screen.height / 2.5f) * ShowCircleSizeModifier;
    }

    // Update is called once per frame
    protected virtual void Update()
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
                    //Go to the action spot
                    GoToAction();
                }
            }
            #endregion

            //If the destination is still this action's target position, it means the player hasn't cancelled the action
            if (_PointAndClickBehaviour.destinationPos == TargetActionPosition.position)
            {
                //If the distance between the player's position and the target position (ajusted to match yy axis values) is next to nothing, the action is performed
                if (Vector3.Distance(new Vector3(TargetActionPosition.position.x, _PlayerVariables.transform.position.y, TargetActionPosition.position.z), _PlayerVariables.transform.position) < 0.1f)
                {
                    PerformAction();
                }
            }
        }
    }

    protected void OnGUI()
    {
        if (PlayerGlobalVariables.instance.ShouldDraw)
        {
            //Set the opacity to this button's target opacity
            GUI.color = new Color(1f, 1f, 1f, _Opacity);

            GUI.DrawTexture(new Rect(new Vector2(//Translate (through the current camera) this object's position into a screen point                    //Adjustment to the button's size
                                                 PlayerGlobalVariables.instance.CurrentCamera.WorldToScreenPoint(transform.position).x                  - _ActualSize.x / 2,
                                                 //Adjust to the UI coordinate system by subtracting the screen point position to the screen's height
                                                 Screen.height - PlayerGlobalVariables.instance.CurrentCamera.WorldToScreenPoint(transform.position).y  - _ActualSize.y / 2
                                                ),
                                     //Draw it with the real-time size
                                     _ActualSize),
                            //Draw it with the real-time texture
                            ButtonTexture);

            //Restore the default opacity
            GUI.color = Color.white;
        }
    }

    protected void ResetCurrentInteractableIfEqual()
    {
        //Reset the current interactable (if it is this object)
        if (GameManager.instance.currentInteractable == gameObject)
        {
            GameManager.instance.currentInteractable = null;
        }
    }

    protected virtual void GoToAction()
    {
        //Set the destination position as the action's target position
        _PointAndClickBehaviour.destinationPos = TargetActionPosition.position;
    }

    protected virtual void PerformAction()
    {
        
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            //If the current interactable hovered is this one
            if (GameManager.instance.currentInteractable == gameObject)
            {
                //Set the current interactable to null
                GameManager.instance.currentInteractable = null;
            }
        }
    }
}
