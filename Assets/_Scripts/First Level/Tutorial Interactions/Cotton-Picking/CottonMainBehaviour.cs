using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class CottonMainBehaviour : MonoBehaviour
{
	public PlayerGlobalVariables PlayerGlobalVariables;
	public LayerMask ObjectLayerMask;
	public LayerMask CottonBasketLayerMask;
	private RaycastHit hitObject;
	public CottonPickUpBehaviour CurrentObject;

	public CottonBasketBehaviour cottonBasket;

	public Interaction Interaction;

	CloseupCameraSelectObject _objectSelectionScript;

	public int requiredAmount;

	public bool SolutionFound;

    //The target timeline's controller
    public PlayableDirector TimelineController;

    //The global FMOD manager
    private FMODLittleGirl fmod;

    // Start is called before the first frame update
    void Start()
	{
		cottonBasket = FindObjectOfType<CottonBasketBehaviour>();
	}

	void Awake()
	{
        fmod = FindObjectOfType<FMODLittleGirl>();
        Interaction = GetComponent<Interaction>();
		PlayerGlobalVariables = FindObjectOfType<PlayerGlobalVariables>();
		cottonBasket = FindObjectOfType<CottonBasketBehaviour>();
		
		Interaction.Started.AddListener(OnInteractionStarted);
		Interaction.Stopped.AddListener(OnInteractionStopped);

        StartCoroutine(CheckSolution());
		//enabled = false;
	}

	void Update()
	{
		//DropWithClick();
		DropWithDrag();
	}

	private void FixedUpdate()
	{
		if (SolutionFound)
		{
			new WaitForSeconds(1);
			Debug.Log("exiting interaction");
			Interaction.ExitInteraction();
		}
		else
		{
			enabled = true;
		}
	}

	IEnumerator CheckSolution()
	{
		bool _check = true;

		while (_check)
		{
			if (cottonBasket.InContact.Count >= requiredAmount)
			{
				SolutionFound = true;
			}
			else
			{
				SolutionFound = false;
			}

			yield return new WaitForSecondsRealtime(0.1f);
		}
	}


	private void OnInteractionStarted()
	{
		StartCoroutine(CheckSolution());

		if (SolutionFound)
		{
			Debug.Log("exiting interaction");
		    Interaction.ExitInteraction();
		}
		else
		{
		    enabled = true;
		}
	}

	private void OnInteractionStopped()
	{
        //Disable the timeline so that it stops
        TimelineController.gameObject.SetActive(true);
        TimelineController.Play();

        //Hide the mouse cursor and drawables
        PlayerGlobalVariables.instance.ShouldDrawCursor = false;
        PlayerGlobalVariables.instance.ShouldDraw = false;

        enabled = false;
	}

	public void DropWithClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Time.timeScale > 0f)
			{
				var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

				if (CurrentObject && Physics.Raycast(ray, out hitObject, 5, CottonBasketLayerMask) && CurrentObject.CanPickUp)
				{
					var basket = hitObject.transform.GetComponent<CottonBasketBehaviour>();
	                
					if (basket)
					{
						var randomPoint = Random.insideUnitCircle * basket.DropRadius;
						var origin = basket.WorldDropPosition;
						origin.x += randomPoint.x;
						origin.z += randomPoint.y;
						CurrentObject.transform.position = origin;
						CurrentObject.GetComponent<Rigidbody>().isKinematic = false;
						CurrentObject.transform.SetParent(basket.transform.GetChild(0));
						CurrentObject.GetComponent<CottonPickUpBehaviour>().CanPickUp = false;
						Destroy(CurrentObject.GetComponent<VisualOnlyDrawable>());
						CurrentObject.GetComponent<SphereCollider>().radius = 0.2f;
						Debug.Log(randomPoint + " | " + origin);
                        CurrentObject = null;   
					}
				}
				else if (Physics.Raycast(ray, out hitObject, 5, ObjectLayerMask))
				{
					CurrentObject = hitObject.transform.GetComponent<CottonPickUpBehaviour>();
				}
				else if (CurrentObject && CurrentObject.CanPickUp)
				{
					CurrentObject.transform.position = CurrentObject.InitialPosition;
					CurrentObject.GetComponent<Rigidbody>().isKinematic = true;
					CurrentObject = null;
				}
			}
		}
	}
	
	public void DropWithDrag()
	{
		if (Input.GetMouseButton(0))
		{
			var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);
			
			//Pick up the item
			if (Physics.Raycast(ray, out hitObject, 5, ObjectLayerMask))
			{
				if (!CurrentObject)
				{
					//Debug.Log(hitObject.collider.name);
					CurrentObject = hitObject.transform.GetComponent<CottonPickUpBehaviour>();
					//CurrentObject.GetComponent<Collider>().enabled = false;
				}

				if (!CurrentObject.CanPickUp)
				{
					CurrentObject = null;
				}
			}

			if (Interaction && CurrentObject)
			{
				var mouseZ = Interaction.InteractionCamera.WorldToScreenPoint(CurrentObject.transform.position).z;
				var mousePosition = Input.mousePosition;
				var offset = CurrentObject.transform.position - Interaction.InteractionCamera.ScreenToWorldPoint(mousePosition);
				mousePosition.z = mouseZ;
				//cursorPosition.z = actualDistance;
				CurrentObject.transform.position = Interaction.InteractionCamera.ScreenToWorldPoint(mousePosition);
				// Debug.Log("offset: " + offset + " mousePosition: " + mousePosition);
				// Debug.Log("mousePositionWorld:" + Interaction.InteractionCamera.ScreenToWorldPoint(mousePosition));	
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			var ray = Interaction.InteractionCamera.ScreenPointToRay(Input.mousePosition);

			if (CurrentObject && Physics.Raycast(ray, out hitObject, 5, CottonBasketLayerMask) && CurrentObject.CanPickUp)
			{
				var basket = hitObject.transform.GetComponent<CottonBasketBehaviour>();
				
				if (basket)
				{
					var randomPoint = Random.insideUnitCircle * basket.DropRadius;
					var origin = basket.WorldDropPosition;
					origin.x += randomPoint.x;
					origin.z += randomPoint.y;
					CurrentObject.transform.position = origin;
					CurrentObject.GetComponent<Rigidbody>().isKinematic = false;
					CurrentObject.transform.SetParent(basket.transform.GetChild(0));
					CurrentObject.GetComponent<CottonPickUpBehaviour>().CanPickUp = false;
					Destroy(CurrentObject.GetComponent<VisualOnlyDrawable>());
					CurrentObject.GetComponent<SphereCollider>().radius = 0.2f;
                    fmod.PlayOneShotAudio("event:/SFX/Puzzles/First Level/Tutorial/SFX_Tutorial_CottonballRip");
                    //Debug.Log(randomPoint + " | " + origin);
                    CurrentObject = null;	
				}
			}
			else if (CurrentObject && CurrentObject.CanPickUp)
			{
				//CurrentObject.GetComponent<Collider>().enabled = true;
				CurrentObject.transform.position = CurrentObject.InitialPosition;
				CurrentObject.GetComponent<Rigidbody>().isKinematic = true;
				CurrentObject = null;
			}
		}
	}
}