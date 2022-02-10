using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaseBehaviour : MonoBehaviour
{
    public bool isOver = false;
    public bool SolutionFound;
    
    public Transform VaseTransform;
    public Transform VaseCentre;
    [Range(-99,0)] public int TargetNumber = 0;
    [Range(-99,0)] public int CurrentNumber = 0;

    private void Start()
    {
        VaseTransform = transform.parent;
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