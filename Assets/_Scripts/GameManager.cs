using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if(_instance != null)
            {
                return _instance;
            }
            else
            {
                //Search for existing instance
                _instance = (GameManager)FindObjectOfType(typeof(GameManager));

                //Create new instance if one doesn't already exist
                if (_instance == null)
                {
                    //Need to create a new GameObject to attach the singleton to
                    var singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<GameManager>();
                    singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";

                    //Make instance persistent
                    DontDestroyOnLoad(singletonObject);
                }

                return _instance;
            }
        }
    }
    
    public LayerMask InteractableMask;
    public Texture2D circleTexture;
    public Texture2D circleOpenTexture;
    public Texture2D squareTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot;
    public GameObject currentInteractable;
    public Camera currentCamera;
    public bool PlayerInRange;

    private float
        _cursorHeightFactor,
        _normalHeightFactor = 10f;
        //_hoverHeightFactor = 1f;
    
    //If there is any need to deactivate GameObjects on Start, this is where they go
    public GameObject[] DeactivateOnStart;

    public List<ReadableProperties> CollectedReadables;

    public List<ReadableProperties> CollectedLetters;
    public List<ReadableProperties> CollectedBooks;
    public List<ReadableProperties> CollectedNewspapers;
    
    public GameObject ReadableMenuItemPrefab;

    public Transform LetterPanel;
    public Transform BookPanel;
    public Transform NewspaperPanel;

    public Camera ReadableCanvasCamera;
    public bool IsInventoryExpanded;

    public InventoryButtonBehaviour InventoryButton;

    public List<ReadableMenuItemBehaviour> ReadableMenuItems;

    [Header("DebugSkip")] 
    public GameObject TutorialParent;
    public Canvas InventoryCanvas;
    public GameObject FirstReadable;
    public ReadableCanvasObjectBehaviour FirstReadableObject;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        //Make sure the time scale is at normal speed
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        hotSpot = new Vector2(circleTexture.width/2, circleTexture.height/2);
        DefineCursorSize();
        Cursor.visible = false;

        //Deactivate all objects that should be deactivated on Start
        if (DeactivateOnStart.Length > 0)
        {
            foreach (GameObject go in DeactivateOnStart)
            {
                go.SetActive(false);
            }
        }
    }

    public void DefineCursorSize()
    {
        _cursorHeightFactor = Screen.height / _normalHeightFactor;
    }

    private void OnGUI()
    {
        if (PlayerGlobalVariables.instance.ShouldDrawCursor)
        {
            if (!currentInteractable)
            {
                GUI.DrawTexture(new Rect(Input.mousePosition.x - _cursorHeightFactor / 2f, Screen.height - Input.mousePosition.y - _cursorHeightFactor / 2f, _cursorHeightFactor, _cursorHeightFactor), circleTexture);
            }
            else
            {
                GUI.DrawTexture(new Rect(Input.mousePosition.x - _cursorHeightFactor / 2f, Screen.height - Input.mousePosition.y - _cursorHeightFactor / 2f, _cursorHeightFactor, _cursorHeightFactor), circleOpenTexture);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DefineCursorSize();

        currentCamera = PlayerGlobalVariables.instance.CurrentCamera;
        
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 9999f, InteractableMask))
        {
            hotSpot = new Vector2(circleTexture.width/2, circleTexture.height/2);
        }

        //TODO: FIND A BETTER WAY TO CHECK IF A CUTSCENE IS PLAYING SO WE DON'T DRAW OVER IT.
        PlayerGlobalVariables.instance.isReading = ReadableCanvasCamera.gameObject.activeSelf;
        //PlayerGlobalVariables.instance.CanMove = !IsInventoryExpanded;

        if (IsInventoryExpanded && !PlayerGlobalVariables.instance.isReading)
        {
            if (Input.GetMouseButtonDown(1))
            {
               InventoryButton.CollapseInventory();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DebugSkip();
        }
    }

    public void CreateContainer(Transform Panel, ReadableProperties obj)
    {
        GameObject newObject = Instantiate(ReadableMenuItemPrefab, transform.position, Quaternion.identity);

        newObject.transform.SetParent(Panel.transform.GetChild(0));

        //Makes sure the position is adjusted to canvas coordinates
        newObject.transform.localPosition = Vector3.zero;

        //Makes sure the scale is properly adapted to the canvas scale
        newObject.transform.localScale = Vector3.one;

        newObject.GetComponent<ReadableMenuItemBehaviour>().RegisterObject(obj.GetComponentInChildren<ReadableObjectBehaviour>());

        ReadableMenuItems.Add(newObject.GetComponent<ReadableMenuItemBehaviour>());
    }

    public void ReorderReadable(int readableType, ReadableProperties readableProperties, int newIndex)
    {
        switch(readableType)
        {
            //If the readable is a letter
            case 0:
                //Remove the old index
                CollectedLetters.Remove(readableProperties);
                //Re-insert the item at the specified index
                CollectedLetters.Insert(newIndex, readableProperties);
                break;
            //If the readable is a book
            case 1:
                //Remove the old index
                CollectedBooks.Remove(readableProperties);
                //Re-insert the item at the specified index
                CollectedBooks.Insert(newIndex, readableProperties);
                break;
            //If the readable is a newspaper
            case 2:
                //Remove the old index
                CollectedNewspapers.Remove(readableProperties);
                //Re-insert the item at the specified index
                CollectedNewspapers.Insert(newIndex, readableProperties);
                break;
        }
    }

    public void DebugSkip()
    {
        TutorialParent.SetActive(false);
        Destroy(TutorialParent);
        FirstReadableObject.DisableReadable(FirstReadable);
        FirstReadable.SetActive(false);
        InventoryCanvas.gameObject.SetActive(true);
        StartCoroutine(WaitBeforeEnablingSkip());
    }

    IEnumerator WaitBeforeEnablingSkip()
    {
        PlayerGlobalVariables.instance.ShouldDraw = true;
        PlayerGlobalVariables.instance.ShouldDrawCursor = true;
        PlayerGlobalVariables.instance.CanMove = true;
        yield return new WaitForEndOfFrame();
    }
}
