using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPiecesBehaviour : MonoBehaviour
{
    public bool SolutionFound;
    public Time Solution;

    // Update is called once per frame
    void Update()
    {
        //If this wood piece's solution was found
        if(SolutionFound)
        {
            //Enable its animator (to play the solution animation)
            GetComponent<Animator>().enabled = true;

            //Deactivate the script
            enabled = false;
        }
    }
    
    [Serializable] public struct Time
    {
        [Range(1,12)]public int Hour;
        [Range(0,59)] public int Minutes;
    }
}
