using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCanvasBehaviour : MonoBehaviour
{ 
    public static InventoryCanvasBehaviour instance;
    public List<ReadableDropBehaviour> Droppables;

    public bool IsDragging = false;

    public Transform DraggingObject = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Canvas>().enabled = !PlayerGlobalVariables.instance.isReading;
    }

    public void RegisterDrag(ReadableDragBehaviour drag)
    {
        drag.OnDragEnd.AddListener(() =>
        {
            foreach (var droppable in Droppables)
            {
                if (droppable.IsPointerOver)
                {
                    droppable.Drop(drag);
                }
            }
        });
    }

    public void RegisterDrop(ReadableDropBehaviour drop)
    {
        Droppables.Add(drop);
        DraggingObject = null;
    }
}
