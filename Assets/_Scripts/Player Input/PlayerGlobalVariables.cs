using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerGlobalVariables : MonoBehaviour
{
    public static PlayerGlobalVariables _instance;
    public static PlayerGlobalVariables instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                //Search for existing instance
                _instance = (PlayerGlobalVariables)FindObjectOfType(typeof(PlayerGlobalVariables));

                //Create new instance if one doesn't already exist
                if (_instance == null)
                {
                    //Need to create a new GameObject to attach the singleton to
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<PlayerGlobalVariables>();
                    singletonObject.name = typeof(PlayerGlobalVariables).ToString() + " (Singleton)";

                    //Make instance persistent
                    DontDestroyOnLoad(singletonObject);
                }

                return _instance;
            }
        }
    }

    //Movement variables
    public float movementSpeed = 40;
    public float lookSpeed = 5;

    //Should the player move?
    public bool CanMove
    {
        get
        {
            return canMove;
        }
        set
        {
            canMove = value;
            //Debug.Log("can move changed to: " + value);
        }
    }
    [SerializeField] private bool canMove;

    //The player's rigidbody
    public Rigidbody _rigidbody;

    //The current active camera
    public Camera CurrentCamera;
    
    public GameObject ReadableUI;
    public GameObject readingInteractionText;
    public bool isReading;
    public GameObject readingTarget;

    //All action drawables in the scene
    public ActionDrawable[] AllActionDrawables;

    //Check if drawables and the cursor should appear
    public bool
        ShouldDraw = false,
        ShouldDrawCursor = false;

    #region Player movement variables
    //The player's navigation agent and point-and-click manager
    private NavMeshAgent _playerNavMeshAgent;
    private PointAndClickBehaviour _playerPointAndClick;

    //The player's navigation agent's speed and acceleration
    private float
        _navMeshAgentSpeed,
        _navMeshAgentAcceleration;
    #endregion

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }

        //Create references for the NavMeshAgent values
        _playerNavMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgentSpeed = _playerNavMeshAgent.speed;
        _navMeshAgentAcceleration = _playerNavMeshAgent.acceleration;
        _playerPointAndClick = GetComponent<PointAndClickBehaviour>();

        //Instance creation
        _rigidbody = GetComponent<Rigidbody>();
        AllActionDrawables = FindObjectsOfType<ActionDrawable>();
        foreach (ActionDrawable action in AllActionDrawables)
        {
            action.enabled = false;
        }
    }

    public void DelayedToggleMovement()
    {
        Invoke("ToggleMovement", 2f);
    }

    public void ToggleMovement()
    {
        CanMove = !CanMove;
    }

    public void ToggleMovement(bool canMove)
    {
        CanMove = canMove;
    }

    public void ChangeMovementSpeed(float speedModifier)
    {
        movementSpeed += speedModifier;
    }

    public void TemporarilyDeactivatePlayerMovement()
    {
        //Set the movement speed to zero
        _playerNavMeshAgent.speed = 0f;
        _playerNavMeshAgent.acceleration = 1000f;
        _playerNavMeshAgent.isStopped = true;

        //Disable the walking animation
        _playerPointAndClick.CanPlayWalkingAnimation = false;
    }

    public void ReactivatePlayerMovement()
    {
        //Reset the movement speed value
        _playerNavMeshAgent.speed = _navMeshAgentSpeed;
        _playerNavMeshAgent.acceleration = _navMeshAgentAcceleration;
        _playerNavMeshAgent.isStopped = false;

        //Enable the walking animation
        _playerPointAndClick.CanPlayWalkingAnimation = true;
    }
}
