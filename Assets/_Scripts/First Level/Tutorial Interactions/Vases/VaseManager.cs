using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Interaction))]
public class VaseManager : BasePuzzle
{
    private FMODLittleGirl fmod;

    public Interaction Interaction;
    public List<VaseBehaviour> VaseList;
    public VaseBehaviour selectedVase;
    public LayerMask InteractionMask;
    public Material defaultMaterial;
    public Material highlightedMaterial;
    [Range(0, 45)]
    public int Tolerance = 5;

    //The target timeline's controller
    public PlayableDirector TimelineController;

    //The vase enabler
    public GameObject DestroyOnSolve;

    //The static vase
    public GameObject CorrectlyPositionedVase;

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

        //Enable the timeline and play it
        TimelineController.gameObject.SetActive(true);
        TimelineController.Play();

        Destroy(DestroyOnSolve);
        //.enabled = true;

        //Hide the mouse cursor and drawables
        PlayerGlobalVariables.instance.ShouldDrawCursor = false;
        PlayerGlobalVariables.instance.ShouldDraw = false;

        //Activate the correctly positioned vase and destroy this one
        CorrectlyPositionedVase.SetActive(true);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Make sure the game isn't paused
            if (Time.timeScale > 0f)
            {
                foreach (var vase in VaseList)
                {
                    if (vase.isOver)
                    {
                        selectedVase = vase;
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            //Make sure the game isn't paused
            if (Time.timeScale > 0f)
            {
                if (selectedVase)
                {
                    //fmod.PlayOneShotAudio("event:/FlowerPotDrop");
                    //selectedVase.GetComponent<MeshRenderer>().material = defaultMaterial;
                }
                CheckSolution();
                selectedVase = null;
            }
        }  

        if (selectedVase && Interaction.IsInteracting)
        {
            //Debug.Log("can rotate");
            RotateHandleByRaycast();
        }
        
    }
        
    private void CheckSolution()
    {
        int numSolutionsFound = 0;
        
        foreach (var vase in VaseList)
        {
            if (Math.Abs(vase.CurrentNumber - vase.TargetNumber) <= Tolerance)
            {
                vase.SolutionFound = true;
                numSolutionsFound++;
            }
            else
            {
                vase.SolutionFound = false;
            }
        }

        if (numSolutionsFound == VaseList.Count)
        {
            Interaction.ExitInteraction();

            //TODO: ANIMATION PLS
            if (transform.parent)
            {
                transform.parent.gameObject.SetActive(false);
            }
            
            Interaction.enabled = false;
        }
    }
    
    private void RotateHandleByRaycast()
    {
        //selectedVase.GetComponent<MeshRenderer>().material = highlightedMaterial;
        RaycastHit hit;
        Ray ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, 1000, InteractionMask.value))
        {
            return;
        }
        
        Vector3 localPoint = (selectedVase.VaseCentre.position - hit.point);
        //Debug.Log(hit.collider.gameObject.name);
        //Debug.Log("parent: " + selectedVase.transform.parent.position);
        //Debug.Log("hit point: " + hit.point);
        //Debug.Log("local point: " + localPoint);
        //localPoint.z = 0;
            
        Vector3 forward = selectedVase.VaseTransform.parent.forward;
        Vector3 right = selectedVase.VaseTransform.parent.right;
        float angleToForward = Mathf.RoundToInt(Vector3.Angle(forward, localPoint));
        //Debug.Log(angleToForward);
        float angleToRight = Mathf.RoundToInt(Vector3.Angle(right, localPoint));
        //Debug.Log(angleToRight);

//working vases

        if (angleToRight >= 90)
        {
            angleToForward = 180 - angleToForward;
        }
        else
        {
            angleToForward = angleToForward + 180;
        }
        
// working safe dials
//        if (localPoint.x > 0.0f)
//        {
//            newAngle = 180 - newAngle;
//        }
//        else
//        {
//            newAngle = newAngle + 180;
//        }

        angleToForward = angleToForward % 360;

        //TODO: FIX INVERTED ANGLE AGAIN
        var normalisedAngle = (1 - angleToForward) / 359;
        var newNumber = Mathf.RoundToInt(normalisedAngle * 99);

        //Válter, this is where the angle change is verified so that the sound effect doesn't spam
        if (selectedVase.VaseTransform.localRotation != Quaternion.Euler(0, -newNumber * 3.6f, 0))
        {
            fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/Tutorial/SFX_Tutorial_VaseDrag");
        }

        selectedVase.VaseTransform.localRotation = Quaternion.Euler(0, angleToForward, 0);
        selectedVase.CurrentNumber = newNumber;
        
    }
    
    public override void Solve()
    {
        base.Solve();
        enabled = true;
        //fmod.PlayOneShotAudio("event:/SafeSolution");
        SolutionFound = true;
        //fmod.PlayOneShotAudio("event:/DoorSolution");
        enabled = false;
    }
}