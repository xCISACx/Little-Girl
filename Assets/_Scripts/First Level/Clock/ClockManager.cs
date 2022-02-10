using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Interaction))]
public class ClockManager : BasePuzzle
{
    private FMODLittleGirl fmod;

    //The puzzle's interaction
    public Interaction Interaction;

    //A list with all the clock hands
    public List<ClockHandBehaviour> ClockHandList;

    //The selected hand
    private ClockHandBehaviour selectedHand;

    //The clock's layer mask
    public LayerMask clockMask;

    //The hour and minute hands
    public GameObject
        hourHand,
        minutesHand;

    //The current hours and minutes
    public int
        currentMinutes,
        currentHour;

    //A list with all the wood pieces (solution informers)
    public List<WoodPiecesBehaviour> WoodPiecesList;

    //The number of individual solutions found
    public int CurrentSolutionIndex;

    //The navigation point from the hallway to the library entrance
    public GameObject OnSolutionNavigationPoint;

    //Check if the puzzle is being skipped
    private bool _isBeingSkipped = true;

    // Start is called before the first frame update
    void Start()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
        Interaction.Started.AddListener(OnInteractionStarted);
        Interaction.Stopped.AddListener(OnInteractionStopped);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {      
        //If the player clicks
        if (Input.GetMouseButtonDown(0))
        {
            //Check which hand the mouse is over
            foreach (var hand in ClockHandList)
            {
                if (hand.isOver)
                {
                    //Set the selected hand
                    selectedHand = hand;
                }
            }
        }
        
        //If the player releases the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            //Set the selected hand as null
            selectedHand = null;

            //Update the hours and minutes
            UpdateTime();

            //Check if a solution was found
            CheckSolution();
        }  

        //If the player is selecting a hand
        if (selectedHand)
        {
            //Rotate the selected hand
            RotateHandleByRaycast();
        }

        //If the solution was found
        if (SolutionFound)
        {
            //The puzzle wasn't debug- or load-skipped
            _isBeingSkipped = false;

            //Solve the puzzle
            Solve();
        }
        
        //Update the hours and minutes
        UpdateTime();
    }

    private void CheckSolution()
    {
        //If the individual solutions found isn't equal to the amount of individual solutions to find
        if (CurrentSolutionIndex < WoodPiecesList.Count)
        {
            //Get the available wood piece (which is the last, because the puzzle's solutions are sequential)
            var woodPiece = WoodPiecesList[CurrentSolutionIndex];
        
            //Check if the current time is equal to the solution's time
            if (CheckTimeSolution(woodPiece.Solution.Hour, woodPiece.Solution.Minutes)) //(currentHour == woodPiece.Solution.Hour && currentMinutes == woodPiece.Solution.Minutes)
            {
                //Set the solution as found
                woodPiece.SolutionFound = true;

                //Increase the solution index
                CurrentSolutionIndex++;

                //If the puzzle isn't finished (if the player hasn't found all solutions)
                if (CurrentSolutionIndex < WoodPiecesList.Count)
                {
                    //Play the solution's resolution sound event
                    fmod.PlayOneShotAudio("event:/SFX/_Stingers/Puzzles/First Level/SFX_Stingers_Puzzle_Clock_IndividualSolution");
                }
            }
            else
            {
                woodPiece.SolutionFound = false;
            }
            
            //Check if the puzzle's solution was found
            SolutionFound = CurrentSolutionIndex >= WoodPiecesList.Count;
            
            if (SolutionFound)
            {   
                //Exit the interaction
                Interaction.ExitInteraction();

                //Activate the door's animator (to play the "open door" animation)
                transform.parent.GetComponent<Animator>().enabled = true;

                //Disable the interaction-enabling collider
                GetComponent<Collider>().enabled = false;

                //Activate the solution-enabled navigation point
                OnSolutionNavigationPoint.SetActive(true);

                //Destroy the action drawable component
                Destroy(transform.parent.GetComponent<ActionDrawable>());

                //Disable the interaction
                Interaction.enabled = false;

                //Play the global solution's sound event
                fmod.PlayOneShotAudio("event:/SFX/_Stingers/Puzzles/First Level/SFX_Stingers_DoorSolution");
            }
        }
    }

    private bool CheckTimeSolution(int solutionHour, int solutionMinutes)
    {
        //Return if the current time is equal to the solution's time
        return currentHour == solutionHour && currentMinutes == solutionMinutes;
    }

    private void RotateHandleByRaycast()
    {
        RaycastHit hit;

        //Get the mouse's pointer position
        Ray ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

        //If the raycast is out of reach, end the function prematurely
        if (!Physics.Raycast(ray, out hit, 1000, clockMask.value))
        {
            return;
        }
        
        //Translate the world point into a local point that starts from the clock
        Vector3 localPoint = (transform.position - hit.point);

        //Reset the local point's zz axis coordinates
        localPoint.z = 0;
            
        //Get the clock's vertical axis
        Vector3 vertical = Vector3.up;
            
        //Get the local point's angle
        float newAngle = Vector3.Angle(vertical, localPoint);
        //Snap the local point's angle
        newAngle = Mathf.Round(-newAngle / 6) * 6;

        //Get a normalised angle
        var normalisedAngle = (360 - Mathf.Abs(newAngle - 180)) / 360;
        var newNumber = Mathf.RoundToInt(normalisedAngle * 180);

        //If the local point's x is below zero, the pointer is on the left side
        if (localPoint.x < 0.0f)
        {
            if (selectedHand.transform.parent.localRotation != Quaternion.Euler(0, 0, -newNumber * 2f))
            {
                //Play the sound event
                if (selectedHand.name.Contains("Minute"))
                {
                    fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/SFX_Puzzles_Clock_MinutePointer");
                }
                else if (selectedHand.name.Contains("Hour"))
                {
                    fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/SFX_Puzzles_Clock_HourPointer");
                }
            }

            selectedHand.transform.parent.localRotation = Quaternion.Euler(0, 0, -newNumber * 2f);
        }
        //If the local point's x is above zero, the pointer is on the right side
        else
        {
            if (selectedHand.transform.parent.localRotation != Quaternion.Euler(0, 0, newNumber * 2f))
            {
                //Play the sound event
                if (selectedHand.name.Contains("Minute"))
                {
                    fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/SFX_Puzzles_Clock_MinutePointer");
                }
                else if (selectedHand.name.Contains("Hour"))
                {
                    fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/SFX_Puzzles_Clock_HourPointer");
                }
            }

            selectedHand.transform.parent.localRotation = Quaternion.Euler(0, 0, newNumber * 2f);
        }
    }

    private void UpdateTime()
    {
        //Translate the hour hand's rotation into hour values
        currentHour = (int)hourHand.transform.localEulerAngles.z / 30;
        
        //Corner-case: if the current hour is 0, set it as 12
        if (currentHour == 0)
        {
            currentHour = 12;
        }
        
        //Translate the minute hand's rotation into minute values
        currentMinutes = (int)((minutesHand.transform.localEulerAngles.z / 60) * 10);
    }

    private void OnInteractionStarted()
    {
        if (SolutionFound)
        {
            Interaction.ExitInteraction();
        }
        else
        {
            enabled = true;
        }
    }

    private void OnInteractionStopped()
    {
        enabled = false;
    }
    
    public override void Solve()
    {
        base.Solve();
        foreach (var woodPiece in WoodPiecesList)
        {
            woodPiece.SolutionFound = true;
        }

        //Get the Animator component from the parent object
        Animator parentAnimator = transform.parent.GetComponent<Animator>();

        //Enable the animator
        parentAnimator.enabled = true;

        if (_isBeingSkipped)
        {
            //Start the animation at a certain time
            parentAnimator.Play("DoorOpen", 0, 2f);

            //Start the wood pieces' animation at a certain time
            foreach(WoodPiecesBehaviour woodPiece in WoodPiecesList)
            {
                woodPiece.GetComponent<Animator>().Play("SymbolClose", 0, 1f);
            }
        }

        //Activate the unlocked navigation point
        OnSolutionNavigationPoint.SetActive(true);
    }
}
