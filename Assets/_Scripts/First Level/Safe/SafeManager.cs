using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Interaction))]
public class SafeManager : BasePuzzle
{
    private FMODLittleGirl fmod;

    //The puzzle's interaction
    public Interaction Interaction;

    //A list with all the dials
    public List<DialBehaviour> DialList;

    //The selected dial
    public DialBehaviour selectedDial;

    //The puzzle's interaction mask
    public LayerMask InteractionMask;

    //The tolerance for each dial's solution
    [Range(0, 5)]
    public int Tolerance = 5;

    //The first level's ending cutscene
    public PlayableDirector ClosingCutscene;
    
    void Start()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
        Interaction = GetComponent<Interaction>();
        Interaction.Started.AddListener(OnInteractionStarted);
        Interaction.Stopped.AddListener(OnInteractionStopped);
        enabled = false;
    }

    public void OnInteractionStarted()
    {
        enabled = true;
    }
    
    public void OnInteractionStopped()
    {
        enabled = false;
    }

    private void Update()
    {
        //If the player clicks
        if (Input.GetMouseButtonDown(0))
        {
            //Check which dial the mouse is over
            foreach (var dial in DialList)
            {
                if (dial.isOver)
                {
                    //Set the selected dial
                    selectedDial = dial;
                }
            }
        }
        
        //If the player releases the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            //Check for solutions
            CheckSolution();

            //Set the selected dial as null
            selectedDial = null;
        }  

        //If the player has a dial selected
        if (selectedDial)
        {
            //Rotate the dial
            RotateHandleByRaycast();
        }
        
        //If the global solution was found
        if (SolutionFound)
        {
            //Solve the puzzle
            Solve();
        }
    }
        
    private void CheckSolution()
    {
        //Reset the number of solutions found
        int numSolutionsFound = 0;
        
        foreach (var dial in DialList)
        {
            //If the dial's current number is close to its target number (taking into account its tolerance)
            if (Math.Abs(dial.CurrentNumber - dial.TargetNumber) <= Tolerance)
            {
                //Set the dial's solution as found
                dial.SolutionFound = true;

                //Increase the solution index
                numSolutionsFound++;
            }
            else
            {
                dial.SolutionFound = false;
            }
        }

        //If the number of solutions found matches the total amount of solutions
        if (numSolutionsFound == DialList.Count)
        {
            //Exit the interaction
            Interaction.ExitInteraction();

            //Set the global solution as "found"
            SolutionFound = true;

            //Disable the action drawable
            GetComponentInParent<ActionDrawable>().enabled = false;
            
            Interaction.enabled = false;
        }
    }
    
    private void RotateHandleByRaycast()
    {
        RaycastHit hit;

        //Get the mouse's pointer position
        Ray ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);
        
        //If the raycast is out of reach, end the function prematurely
        if (!Physics.Raycast(ray, out hit, 1000, InteractionMask.value))
        {
            return;
        }

        //Translate the world point into a local point that starts from the dial
        Vector3 localPoint = (selectedDial.DialCentre.position - hit.point);

        //Reset the local point's zz axis coordinates
        localPoint.z = 0;
            
        //Get the dial's axis
        Vector3 forward = selectedDial.DialTransform.parent.forward;
        Vector3 right = selectedDial.DialTransform.parent.right;
        Vector3 vertical = Vector3.up;
            
        //Get the local point's angle
        float newAngle = Mathf.RoundToInt(Vector3.Angle(forward, localPoint));

        //If the local point's x is below zero, the pointer is on the left side
        if (localPoint.x > 0.0f)
        {
            newAngle = 180 - newAngle;
        }
        //If the local point's x is above zero, the pointer is on the right side
        else
        {
            newAngle = newAngle + 180;
        }

        newAngle = newAngle % 360;
        
        var normalisedAngle = (359 - Mathf.Abs(newAngle)) / 359;
        var newNumber = Mathf.RoundToInt(normalisedAngle * 99);
        
        //Check if the dial's rotation changed and play the sound event if it did
        if (selectedDial.DialTransform.localRotation != Quaternion.Euler(0, -newNumber * 3.6f, 0))
        {
            fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/SFX_Puzzles_SafeDial");
        }

        //Set the dial's rotation
        selectedDial.DialTransform.localRotation = Quaternion.Euler(0, - newNumber * 3.6f, 0);

        //Update the dial's current number to match its rotation
        selectedDial.CurrentNumber = newNumber;
    }
    
    public override void Solve()
    {
        base.Solve();
        foreach (var dial in DialList)
        {
            dial.CurrentNumber = dial.TargetNumber;
        }
        
        fmod.PlayOneShotAudio("event:/Stingers/Puzzles/First Level/SFX_Stingers_Puzzle_Safe_Solution");

        //Set the global solution as "found"
        SolutionFound = true;

        //Hide the player
        PlayerGlobalVariables.instance.gameObject.SetActive(false);

        //Hide the user interface
        PlayerGlobalVariables.instance.ShouldDrawCursor = false;
        PlayerGlobalVariables.instance.ShouldDraw = false;
        GameObject.FindGameObjectWithTag("Inventory").SetActive(false);
        
        //Play the closing cutscene
        ClosingCutscene.gameObject.SetActive(true);
        ClosingCutscene.Play();

        //Disable the script
        enabled = false;
    }
}
