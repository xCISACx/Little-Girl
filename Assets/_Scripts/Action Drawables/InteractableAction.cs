using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableAction : ActionDrawable
{
    protected override void PerformAction()
    {
        //Enable the interaction
        if (GetComponentInChildren<BasePuzzle>())
        {
            //If the puzzle's solution wasn't found
            if (!GetComponentInChildren<BasePuzzle>().SolutionFound)
            {
                //Start the interaction
                GetComponentInChildren<Interaction>().StartInteraction();
            }
            //If the solution was already found
            else
            {
                //Deactivate the drawable
                enabled = false;
            }
        }
    }
}
