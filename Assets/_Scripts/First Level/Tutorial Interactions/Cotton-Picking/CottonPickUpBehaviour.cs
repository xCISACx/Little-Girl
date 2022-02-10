using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CottonPickUpBehaviour : MonoBehaviour
{
    public Vector3 InitialPosition;
    public bool CanPickUp;
    
    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
