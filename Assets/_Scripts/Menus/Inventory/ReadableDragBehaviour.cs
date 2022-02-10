using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ReadableDragBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool IsDragging;
    public UnityEvent OnDragEnd;
    public Transform Container;

    public RectTransform
        //The readable's name
        Name,
        //The readable's reorder sprite
        ReorderSprite;

    private Vector2
        //The name's initial position
        _nameInitialPosition,
        //The name's position when the item is selected
        _nameIndentedPosition,
        //The sprite's initial position
        _spriteInitialPosition,
        //The sprite's position when the item is selected
        _spriteIndentedPosition;

    private void Awake()
    {
        //Save the initial position for the name and the sprite
        _nameInitialPosition = Name.localPosition;
        _spriteInitialPosition = ReorderSprite.localPosition;

        //Calculate the indented position for the name and the sprite
        _nameIndentedPosition = _nameInitialPosition + new Vector2(15, 0);
        _spriteIndentedPosition = _spriteInitialPosition + new Vector2(30, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        InventoryCanvasBehaviour.instance.RegisterDrag(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IndentItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        InventoryCanvasBehaviour.instance.IsDragging = true;
        InventoryCanvasBehaviour.instance.DraggingObject = transform.parent;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryCanvasBehaviour.instance.IsDragging = false;
        InventoryCanvasBehaviour.instance.DraggingObject = null;
        UnindentItem();
        OnDragEnd.Invoke();
    }

    private void IndentItem()
    {
        Name.localPosition = _nameIndentedPosition;
        ReorderSprite.localPosition = _spriteIndentedPosition;
    }

    private void UnindentItem()
    {
        Name.localPosition = _nameInitialPosition;
        ReorderSprite.localPosition = _spriteInitialPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IndentItem();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UnindentItem();
    }
}
