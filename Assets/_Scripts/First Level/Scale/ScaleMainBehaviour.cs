using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ScaleMainBehaviour : BasePuzzle
{
	private FMODLittleGirl fmod;

    //The global player variables
	public PlayerGlobalVariables PlayerGlobalVariables;
    
	public LayerMask
        //The layer mask in which the objects are
        ObjectLayerMask,
        //The layer mask in which the scale's plates are
        ScalePlatesLayerMask;

    //The object hit by the player's click
	private RaycastHit hitObject;

    //The current object ready to be positioned
	public ScalePickUpBehaviour CurrentObject;

	Transform
		//The scale's top
		_scaleTop,
		//The left plate
		_leftPlate,
		//The right plate
		_rightPlate;

	Vector3
		//The left scale's target position
		_leftTargetPosition,
		//The right scale's target position
		_rightTargetPosition;

	[SerializeField]
	//The total height the plates can travel
	private float _height;

    //The puzzle's interaction
	public Interaction Interaction;

	public float
		//The total mass currently in the scale
		TotalMass = 1,
		//The mass currently in the left plate
		LeftPlateMass,
		//The mass currently in the right plate
		RightPlateMass;

	private ScalePlateBehaviour
		//The left plate's script (to get mass)
		_leftPlateScript,
		//The right plate's script (to get mass)
		_rightPlateScript;

    //The office door
	public GameObject OfficeDoor;

    //The solution index
	private int _solutionCount;

    //The margin of error taken into account when calculating plate mass difference
    public float MarginOfError = 0.1f;

    //The navigation point from the library to the office
    public GameObject OnSolutionNavigationPoint;

    //The office door's lock
    public LockBoltBehaviour LockBolt;

    //The scale objects' parent
    public GameObject ScaleObjectsParent;

    //A list with all scale objects
    public List<ScalePickUpBehaviour> ScaleObjects;

    //A list with all scale objects' rigidbodies
    [SerializeField]
    private List<Rigidbody> _scaleObjectsRB = new List<Rigidbody>();

    //Store the last solution-checking coroutine started
    private Coroutine _lastCoroutine;

    // Start is called before the first frame update
    void Start()
	{
		//Instance creation
		_scaleTop = transform.GetChild(0);
		_leftPlate = GameObject.FindGameObjectWithTag("LeftPlate").transform;
		_rightPlate = GameObject.FindGameObjectWithTag("RightPlate").transform;
		_height = Vector3.Distance(transform.position, _scaleTop.position);

		_leftPlateScript = GameObject.FindGameObjectWithTag("LeftPlate").GetComponent<ScalePlateBehaviour>();
		_rightPlateScript = GameObject.FindGameObjectWithTag("RightPlate").GetComponent<ScalePlateBehaviour>();

		ScaleObjects = FindObjectsOfType<ScalePickUpBehaviour>().ToList();
        foreach (ScalePickUpBehaviour obj in ScaleObjects)
        {
            if (obj.GetComponent<Rigidbody>() != null)
            {
                _scaleObjectsRB.Add(obj.GetComponent<Rigidbody>());
            }
        }
	}

	void Awake()
	{
		fmod = FindObjectOfType<FMODLittleGirl>();

		Interaction = GetComponent<Interaction>();
		PlayerGlobalVariables = FindObjectOfType<PlayerGlobalVariables>();
		
		_leftPlateScript = GameObject.FindGameObjectWithTag("LeftPlate").GetComponent<ScalePlateBehaviour>();
		_rightPlateScript = GameObject.FindGameObjectWithTag("RightPlate").GetComponent<ScalePlateBehaviour>();
		
		Interaction.Started.AddListener(OnInteractionStarted);
		Interaction.Stopped.AddListener(OnInteractionStopped);
		enabled = false;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
        //If the total mass is above zero, there are objects on the plates
		if (TotalMass > 0f)
		{
            //Set the left plate's height according to its weight percentage
			_leftPlate.GetComponent<ScalePlateBehaviour>().TargetPosition =
				new Vector3(-0.7f, ((LeftPlateMass / TotalMass) - 0.5f) * _height, 0f);

            //Set the right plate's height according to its weight percentage
            _rightPlate.GetComponent<ScalePlateBehaviour>().TargetPosition =
				new Vector3(0.7f, (0f - (LeftPlateMass / TotalMass) + 0.5f) * _height, 0f);
		}
        //If the total mass is zero, there are no objects on the plates, and only the two weights are taken into account
		else
		{
            //Set the plates to their default positions (plate with weights is all the way down)
			_leftPlate.GetComponent<ScalePlateBehaviour>().TargetPosition = new Vector3(-0.7f, 0.225f, 0f);
			_rightPlate.GetComponent<ScalePlateBehaviour>().TargetPosition = new Vector3(0.7f, -0.225f, 0f);
		}

        //Set mass values to match the objects on top of the plates
		LeftPlateMass = _leftPlateScript.Mass;
		RightPlateMass = _rightPlateScript.Mass;

        //Calculate the total mass of the plates
		TotalMass = LeftPlateMass + RightPlateMass;

        //If the solution countdown is bigger than four, the puzzle was solved
		if (_solutionCount > 4)
		{
            //Play the solution sound event
			fmod.PlayOneShotAudio("event:/SFX/_Stingers/Puzzles/First Level/SFX_Stingers_DoorSolution");

            //Activate the office door's animator (play the "door open" animation)
            OfficeDoor.GetComponent<Animator>().enabled = true;

            //Exit the interaction
            Interaction.ExitInteraction();

            //Disable the interaction
            GetComponent<Interaction>().enabled = false;

            //Set the solution as "found"
            SolutionFound = true;

            //Destroy the action drawable component
            Destroy(GetComponentInParent<ActionDrawable>());

            //Activate the solution-enabled navigation point
            OnSolutionNavigationPoint.SetActive(true);

            //Disable this script
            enabled = false;
        }
	}

	void Update()
	{
		//DropWithClick();
		DropWithDrag();

        //Set the lock bolt's rotation according to the mass difference between the plates
        LockBolt.TargetAngleAmount = _leftPlateScript.Mass / TotalMass;
	}

	IEnumerator CheckSolution()
	{
		bool _check = true;

        //Loop
		while (_check)
		{
            //If the left plate has more than two objects in contact and the right plate has more than one
			if (_leftPlateScript.InContact.Count > 2 && _rightPlateScript.InContact.Count > 0)
			{
                //If the mass difference percentage is approximately equal to 50% (minus margin of error)
                if(_leftPlateScript.Mass / TotalMass > 0.5f - MarginOfError)
				{
                    //If the mass difference percentage is approximately equal to 50% (plus margin of error)
                    if (_leftPlateScript.Mass / TotalMass < 0.5f + MarginOfError)
                    {
                        //Increase the solution count
                        _solutionCount++;
                    }
				}
			}
			else
			{
                //Reset the solution count
				_solutionCount = 0;
			}

			yield return new WaitForSecondsRealtime(0.5f);
		}
	}


	private void OnInteractionStarted()
	{
        //If there was a previously started coroutine
        if(_lastCoroutine != null)
        {
            //Stop the previous coroutine
            StopCoroutine(_lastCoroutine);
        }

        //Start a coroutine to check the solution
		_lastCoroutine = StartCoroutine(CheckSolution());

		enabled = true;
	}

	private void OnInteractionStopped()
	{
        //Reset all scale objects to their initial positions and rotations
		foreach (var scaleObject in ScaleObjects)
		{
			scaleObject.transform.position = scaleObject.InitialPosition;
            scaleObject.transform.eulerAngles = scaleObject.InitialRotation;
        }
	}

	public void DropWithClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

			//Dropping the object
			if (CurrentObject && Physics.Raycast(ray, out hitObject, 5, ScalePlatesLayerMask) && CurrentObject.CanPickUp)
			{
				fmod.PlayOneShotAudio("event:/Scale_DropItem"); // play audio shot
				var plate = hitObject.transform.GetComponent<ScalePlateBehaviour>();
				var randomPoint = Random.insideUnitCircle * plate.DropRadius;
				var origin = plate.WorldDropPosition;
				origin.x += CurrentObject.DroppedPosition.x;
				origin.z += CurrentObject.DroppedPosition.y;
				CurrentObject.transform.position = origin;
				CurrentObject.transform.SetParent(plate.transform);
				//Debug.Log(randomPoint + " | " + origin);
				CurrentObject = null;
			}
			
			//Pick up the item
			else if (Physics.Raycast(ray, out hitObject, 5, ObjectLayerMask))
			{
				//Debug.Log(hitObject.collider.name);
				CurrentObject = hitObject.transform.GetComponent<ScalePickUpBehaviour>();
				fmod.PlayOneShotAudio("event:/Scale_PickItem"); // play audio shot
			}
			
			//Put item back
			else if (CurrentObject && CurrentObject.CanPickUp)
			{
				CurrentObject.transform.position = CurrentObject.InitialPosition;
				CurrentObject.transform.eulerAngles = CurrentObject.InitialRotation;
				CurrentObject = null;
			}
		}
	}
	
	public void DropWithDrag()
	{
        //If the player clicks
		if (Input.GetMouseButton(0))
		{
			var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);
			
			//Pick up the item
			if (Physics.Raycast(ray, out hitObject, 5, ObjectLayerMask))
			{
                //If there is no currently picked up object
				if (!CurrentObject)
				{
                    //Set the current object as the hit object
					CurrentObject = hitObject.transform.GetComponent<ScalePickUpBehaviour>();
				}
			}

            //If there is a current object
			if (Interaction && CurrentObject)
			{
                //Calculate the target mouse position and zz axis coordinate
				var mouseZ = Interaction.InteractionCamera.WorldToScreenPoint(CurrentObject.transform.position).z;
				var mousePosition = Input.mousePosition;
				mousePosition.z = mouseZ;

                //Set the currently picked up object's position as the mouse's position
				CurrentObject.transform.position = Interaction.InteractionCamera.ScreenToWorldPoint(mousePosition);

                //Deactivate all other objects' rigidbodies so that this object cannot push others
                ToggleOtherObjectsRigidbody(false, CurrentObject.ObjectsRigidbody);
            }
		}

        //If the player releases the mouse button
		if (Input.GetMouseButtonUp(0))
		{
			var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);
			
            //If there is a current (and movable) object and the mouse is over a scale plate
			if (CurrentObject && Physics.Raycast(ray, out hitObject, 5, ScalePlatesLayerMask) && CurrentObject.CanPickUp)
			{
                //Play the object's dropping sound event
				fmod.PlayOneShotAudio(CurrentObject.DropSoundEventName);

                //Get the target plate's script
				var plate = hitObject.transform.GetComponent<ScalePlateBehaviour>();

                //Get the origin from which the object's position will be calculated (the plate)
				var origin = plate.WorldDropPosition;

                //Add the target position values to the coordinates
				origin.x = CurrentObject.DroppedPosition.x;
				origin.z = CurrentObject.DroppedPosition.z;

                //Set the object as a child of the target plate
				CurrentObject.transform.SetParent(plate.transform);

                //Set the object's drop position
				origin.y = -plate.DropPosition.y - 1;

                //Set the position as the target dropped position
                CurrentObject.transform.localPosition = origin;

                //Set the rotation as the target dropped rotation
                CurrentObject.transform.localEulerAngles = CurrentObject.DroppedRotation;
                
                //Reset the current object to null
				CurrentObject = null;
			}
            //If the mouse isn't over a scale plate
			else if (CurrentObject && CurrentObject.CanPickUp)
			{
                //Set the object as a child of the scale objects' parent
				CurrentObject.transform.SetParent(ScaleObjectsParent.transform);

                //Set the position as the target initial position
				CurrentObject.transform.position = CurrentObject.InitialPosition;

                //Set the position as the target initial position
				CurrentObject.transform.localEulerAngles = CurrentObject.InitialRotation;

                //Reset the current object to null
                CurrentObject = null;
			}

            //Reenable object movement
            ToggleOtherObjectsRigidbody(true);
		}
	}

    private void ToggleOtherObjectsRigidbody(bool componentState)
    {
        //For all the scale objects' rigidbodies
        foreach (Rigidbody rb in _scaleObjectsRB)
        {
            //If the rigidbody should be disabled
            if (componentState == false)
            {
                //Freeze all of the rigidbody's axis
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            //If the rigidbody should be enabled
            else
            {
                //Freeze the rigidbody's rotation axis only
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    private void ToggleOtherObjectsRigidbody(bool componentState, Rigidbody exception)
    {
        //For all the scale objects' rigidbodies
        foreach (Rigidbody rb in _scaleObjectsRB)
        {
            //If the rigidbody isn't the exception
            if (rb != exception)
            {
                //If the rigidbody should be disabled
                if (componentState == false)
                {
                    //Freeze all of the rigidbody's axis
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                //If the rigidbody should be enabled
                else
                {
                    //Freeze the rigidbody's rotation axis only
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
        }
    }


    public override void Solve()
	{
		base.Solve();

        //Get the Animator component from the door's object
        Animator officeDoorAnimator = OfficeDoor.GetComponent<Animator>();

        //Enable the animator
        officeDoorAnimator.enabled = true;

        //Start the animation at a certain frame
        officeDoorAnimator.Play("OfficeDoorOpen", 0, 1f);

        //Stop the lock bolt's rotation update
        LockBolt.ShouldUpdateRotation = false;

        //Set the lock bolt's rotation to fully vertical
        LockBolt.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);

        //Set the solution as "found"
        SolutionFound = true;

        //Activate the solution-enabled navigation point
		OnSolutionNavigationPoint.SetActive(true);

        //Deactivate this script
		enabled = false;
	}
}