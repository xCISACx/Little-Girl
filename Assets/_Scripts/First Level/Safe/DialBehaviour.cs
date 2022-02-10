using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialBehaviour : MonoBehaviour
{
    //Check if the mouse is over this dial
    public bool isOver = false;

    //Check if this dial's solution was found
    public bool SolutionFound;
    
    //The dial's rotation transforms
    public Transform
        DialTransform,
        DialCentre;

    //The dial's target number
    [Range(0,99)] public int TargetNumber = 0;

    //The dial's current number
    [Range(0,99)] public int CurrentNumber = 0;

    private void Start()
    {
        DialTransform = transform.parent;
    }
    
    private void OnMouseOver()
    {
        isOver = true;
    }

    private void OnMouseExit()
    {
        isOver = false;
    }
}
