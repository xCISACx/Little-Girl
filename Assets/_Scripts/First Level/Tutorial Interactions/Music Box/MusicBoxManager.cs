using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Interaction))]
public class MusicBoxManager : BasePuzzle
{
    private FMODLittleGirl fmod;
    public Interaction Interaction;
    public List<KeyBehaviour> KeyList;
    public KeyBehaviour selectedKey;
    public LayerMask InteractionMask;
    public Material defaultMaterial;
    public Material highlightedMaterial;
    [Range(0, 5)]
    public int Tolerance = 5;

    //The amount of times the user needs to rotate the key correctly
    public int RotationsNeeded = 50;

    //The actual rotation count
    private int _rotations = 0;

    //The second part of the interaction
    public GameObject Part2;

    //The second part's key
    public Transform Part2Key;

    void Start()
    {
        fmod = FindObjectOfType<FMODLittleGirl>();
        Interaction = GetComponent<Interaction>();
        Interaction.Started.AddListener(OnInteractionStarted);
        Interaction.Stopped.AddListener(OnInteractionStopped);
    }

    public void OnInteractionStarted()
    {
        enabled = true;
    }
    
    public void OnInteractionStopped()
    {
        //Get the current xx (xx global, yy local) rotation of the key
        float part2KeyXRotation = KeyList[0].transform.parent.localEulerAngles.y;

        //Add the xx rotation to the second part's key
        Part2Key.eulerAngles -= new Vector3(part2KeyXRotation, 0f, 0f);

        //Activate the second part of the interaction
        Part2.SetActive(true);

        //Deactivate this part's music box object
        transform.parent.parent.parent.gameObject.SetActive(false);
        
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var key in KeyList)
            {
                if (key.isOver)
                {
                    selectedKey = key;
                    //fmod.PlayOneShotAudio("event:/SafeDialInteract");
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedKey)
            {
                //fmod.PlayOneShotAudio("event:/SafeDialInteract_Leaving");
                //selectedDial.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
            CheckSolution();
            selectedKey = null;
        }  

        if (selectedKey && Interaction.IsInteracting)
        {
            //Debug.Log("can rotate");
            RotateHandleByRaycast();
        }
        
    }
        
    private void CheckSolution()
    {
        if (_rotations >= RotationsNeeded)
        {
            Interaction.ExitInteraction();
            SolutionFound = true;
            if (transform.parent)
            {
                transform.parent.gameObject.SetActive(false);
            }
            
            Interaction.enabled = false;
        }
    }
    
    private void RotateHandleByRaycast()
    {
        int previousNumber = selectedKey.CurrentNumber;

        //selectedDial.GetComponent<MeshRenderer>().material = highlightedMaterial;
        RaycastHit hit;
        Ray ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, 1000, InteractionMask.value))
        {
            return;
        }
        
        Vector3 localPoint = (selectedKey.KeyCentre.position - hit.point);
        localPoint.z = 0;
            
        Vector3 forward = selectedKey.KeyTransform.parent.forward;
        Vector3 right = selectedKey.KeyTransform.parent.right;
        Vector3 vertical = Vector3.up;
            
        float newAngle = Mathf.RoundToInt(Vector3.Angle(forward, localPoint));

        if (localPoint.x > 0.0f)
        {
            newAngle = 180 - newAngle;
        }
        else
        {
            newAngle = newAngle + 180;
        }

        newAngle = newAngle % 360;
        
        var normalisedAngle = (359 - Mathf.Abs(newAngle)) / 359;
        var newNumber = Mathf.RoundToInt(normalisedAngle * 25);

        if (_rotations < RotationsNeeded)
        {
            selectedKey.KeyTransform.localRotation = Quaternion.Euler(0, -newNumber * 3.6f * 4f, 0);
        }
        selectedKey.CurrentNumber = newNumber;

        if(selectedKey.CurrentNumber < previousNumber)
        {
            //Increment the rotation count
            _rotations++;

            //TODO: Add a music box key rotation sound effect
            if (_rotations < RotationsNeeded)
            {
                fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/Tutorial/SFX_Tutorial_MusicBoxGear");
            }
            else if(_rotations == RotationsNeeded)
            {
                fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/Tutorial/SFX_Tutorial_MusicBoxComplete");
            }
        }
    }
}
