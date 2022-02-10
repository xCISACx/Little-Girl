using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualOnlyDrawable : ActionDrawable
{
    // Update is called once per frame
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
            }
            //If the mouse is inside the circle that shows the button and outside the button
            else if (_DistanceToMouse <= _DistanceNeededToShow.x / 2f && _DistanceToMouse > _ActualSize.x / 2f)
            {
                //Change the button's texture and size to "normal"
                ButtonTexture = NormalButtonTexture;
                _ActualSize = _NormalSize;
            }
            //If the mouse is inside the button
            else if (_DistanceToMouse <= _ActualSize.x / 2f)
            {
                //Change the button's texture and size to "hover"
                ButtonTexture = HoverButtonTexture;
                _ActualSize = _HoverSize;
            }
            #endregion
        }
    }

    protected override void GoToAction()
    {

    }

    protected override void PerformAction()
    {
        
    }
}
