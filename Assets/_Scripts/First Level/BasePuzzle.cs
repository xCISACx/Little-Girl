using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePuzzle : MonoBehaviour
{
    //Check if the solution was found
    public bool SolutionFound = false;

    //Store the puzzle's ID
    public string ID => gameObject.name;

    public virtual void Solve()
    {
        //Exit the interaction
        GetComponent<Interaction>().ExitInteraction();

        //Disable the interaction-enabling collider
        GetComponent<Collider>().enabled = false;

        //Destroy the action drawable component
        Destroy(transform.parent.GetComponent<ActionDrawable>());

        //Disable the interaction
        GetComponent<Interaction>().enabled = false;
    }
}
