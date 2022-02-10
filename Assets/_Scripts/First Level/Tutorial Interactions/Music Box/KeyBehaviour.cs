using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBehaviour : MonoBehaviour
{
    public bool isOver = false;
    public bool SolutionFound;
    
    public Transform KeyTransform;
    public Transform KeyCentre;
    [Range(0,99)] public int TargetNumber = 0;
    [Range(0,99)] public int CurrentNumber = 0;

    private void Start()
    {
        KeyTransform = transform.parent;
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
