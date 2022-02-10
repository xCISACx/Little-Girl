using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Icon;
    public Sprite ExpandedIcon;
    public Sprite NotExpandedIcon;
    
    public Animator Animator;
    public bool Expanded;

    //Check if the player was able to move before activating the inventory
    private bool _wasMovingBefore;

    //The tab manager
    public TabGroupBehaviour _tabManager;

    //The player's navigation agent and point-and-click manager
    private NavMeshAgent _playerNavMeshAgent;
    private PointAndClickBehaviour _playerPointAndClick;

    //The player's navigation agent's speed and acceleration
    private float
        _navMeshAgentSpeed,
        _navMeshAgentAcceleration;

    //The puzzle scripts
    public BasePuzzle[] LevelPuzzles;
    
    [SerializeField]
    private bool[]
        //Check each puzzle script's state
        _levelPuzzleStates,
        //Check if the player was interacting with the puzzle
        _wasInteractingWithPuzzle;

    [SerializeField]
    //The active camera before activating a readable
    private Camera _cameraPriorToReadable;

    // Start is called before the first frame update
    void Start()
    {
        //Instance creation
        Animator = transform.parent.GetComponent<Animator>();
        _playerNavMeshAgent = PlayerGlobalVariables.instance.GetComponent<NavMeshAgent>();
        _navMeshAgentSpeed = _playerNavMeshAgent.speed;
        _navMeshAgentAcceleration = _playerNavMeshAgent.acceleration;
        _playerPointAndClick = PlayerGlobalVariables.instance.GetComponent<PointAndClickBehaviour>();
        _levelPuzzleStates = new bool[LevelPuzzles.Length];
        _wasInteractingWithPuzzle = new bool[LevelPuzzles.Length];
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.IsInventoryExpanded = Expanded;
    }

    public void OnClick()
    {
        if (!Expanded)
        {
            ExpandInventory();
        }
        else
        {
            CollapseInventory();
        }

        // transform.GetComponent<RectTransform>().anchoredPosition -= Vector2.right;
        //
        // Vector3.MoveTowards(transform.GetComponent<RectTransform>().anchoredPosition, RectTransformUtility.WorldToScreenPoint(Camera.main, DesiredPostition),
        //         2f);
    }

    private void ExpandInventory()
    {
        //Reset animation if needed and start it (again)
        Animator.ResetTrigger("slide in");
        Animator.SetTrigger("slide out");

        //Disable drawables
        PlayerGlobalVariables.instance.ShouldDraw = false;

        //Check if the player could move before opening the inventory
        _wasMovingBefore = PlayerGlobalVariables.instance.CanMove;
        //Disable explicit movement
        PlayerGlobalVariables.instance.CanMove = false;

        //Deactivate any movement
        PlayerGlobalVariables.instance.TemporarilyDeactivatePlayerMovement();

        //If there are any puzzles to disable
        if(LevelPuzzles.Length > 0)
        {
            DeactivatePuzzles();
        }

        Icon.sprite = ExpandedIcon;

        Expanded = true;
    }

    public void CollapseInventory()
    {
        //If there are any tabs open, close them before closing the inventory and reset the active tab
        _tabManager.SetActiveTab();

        //Reset animation if needed and start it (again)
        Animator.ResetTrigger("slide out");
        Animator.SetTrigger("slide in");

        //Reenable drawables
        PlayerGlobalVariables.instance.ShouldDraw = true;

        //If the player was able to move before opening the inventory, allow them to move again
        if (_wasMovingBefore)
        {
            PlayerGlobalVariables.instance.CanMove = true;
        }

        //Reactivate player movement (to pick up last destination given)
        PlayerGlobalVariables.instance.ReactivatePlayerMovement();

        //If there are puzzles to reactivate
        if (LevelPuzzles.Length > 0)
        {
            ReactivatePuzzles();
        }

        Icon.sprite = NotExpandedIcon;

        Expanded = false;
    }

    private void DeactivatePuzzles()
    {
        //Disable all puzzles
        for(int i = 0; i < LevelPuzzles.Length; i++)
        {
            _levelPuzzleStates[i] = LevelPuzzles[i].enabled;
            _wasInteractingWithPuzzle[i] = LevelPuzzles[i].GetComponent<Interaction>().IsInteracting;

            if(_wasInteractingWithPuzzle[i])
            {
                _cameraPriorToReadable = LevelPuzzles[i].GetComponent<Interaction>().mainCamera;
            }

            LevelPuzzles[i].enabled = false;
        }
    }

    private void ReactivatePuzzles()
    {
        //Enable all puzzles that were active before
        for (int i = 0; i < LevelPuzzles.Length; i++)
        {
            if (_levelPuzzleStates[i])
            {
                LevelPuzzles[i].enabled = true;

                if (_wasInteractingWithPuzzle[i])
                {
                    //Reset the interacting bool to indicate that the player was interacting with the puzzle
                    LevelPuzzles[i].GetComponent<Interaction>().IsInteracting = true;
                    LevelPuzzles[i].GetComponent<Interaction>().mainCamera = _cameraPriorToReadable;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //When the mouse hovers the inventory button, set the current interactable (to avoid movement)
        GameManager.instance.currentInteractable = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //When the mouse exits the inventory button, reset the current interactable
        GameManager.instance.currentInteractable = null;
    }
}
