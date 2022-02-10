using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ReadableDropBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsPointerOver;
    public UnityEvent OnDrop;
    public Transform Container;

    private ReadableProperties _readableProperties;
    
    // Start is called before the first frame update
    void Start()
    {
        InventoryCanvasBehaviour.instance.RegisterDrop(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOver = true;

        //Check if the dragged object exists
        if (InventoryCanvasBehaviour.instance.DraggingObject != null)
        {
            //Check if the dragged object isn't this one
            if (InventoryCanvasBehaviour.instance.DraggingObject != transform)
            {
                //If the dragged object's index is bigger than this object's index, it's below this object
                if (InventoryCanvasBehaviour.instance.DraggingObject.GetSiblingIndex() > transform.GetSiblingIndex())
                {
                    //Activate the top indicator
                    ActivateIndicators(transform.GetChild(3).gameObject);
                    //transform.GetChild(3).gameObject.SetActive(true);
                }
                //If not, it's above this object
                else
                {
                    //Activate the bottom indicator
                    ActivateIndicators(transform.GetChild(2).gameObject);
                    //transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOver = false;

        //Makes sure both reorder indicators are deactivated
        DeactivateIndicators();
    }

    public void Drop(ReadableDragBehaviour currentDraggable)
    {
        _readableProperties = currentDraggable.Container.GetComponent<ReadableMenuItemBehaviour>().ReadableObject.GetComponentInParent<ReadableProperties>();

        OnDrop.Invoke();
        var draggableIndex = currentDraggable.Container.GetSiblingIndex();
        var droppableIndex = Container.GetSiblingIndex();

        currentDraggable.Container.SetSiblingIndex(droppableIndex);

        //Set the readable properties index in the game manager as the new index
        GameManager.instance.ReorderReadable((int) _readableProperties.Type, _readableProperties, droppableIndex);

        DeactivateIndicators();
    }

    private void ActivateIndicators(GameObject targetIndicator)
    {
        targetIndicator.SetActive(true);
    }

    private void DeactivateIndicators()
    {
        //Makes sure both reorder indicators are deactivated
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
    }
}
