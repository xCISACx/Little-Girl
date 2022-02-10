using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHandBehaviour : MonoBehaviour
{
    //Check if the mouse is over this hand
    public bool isOver = false;
    
    private void OnMouseOver()
    {
        isOver = true;
    }

    private void OnMouseExit()
    {
        isOver = false;
    }
}
