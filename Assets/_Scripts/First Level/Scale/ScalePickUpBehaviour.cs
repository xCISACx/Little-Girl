using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePickUpBehaviour : ScaleObject
{
    public Vector3
        //This object's initial position
        InitialPosition,
        //This object's position when dropped on a plate
        DroppedPosition,
        
        //This object's initial rotation
        InitialRotation,
        //This object's rotation when dropped on a plate
        DroppedRotation;

    //Check if this object can be picked up
    public bool CanPickUp;

    //This object's rigidbody
    public Rigidbody ObjectsRigidbody;

    //The name of this object's sound event when dropped on a plate
    public string DropSoundEventName;
    
    // Start is called before the first frame update
    void Start()
    {
        //Store the initial position and rotation
        InitialPosition = transform.position;
        InitialRotation = transform.localEulerAngles;

        //Get the object's rigidbody
        ObjectsRigidbody = GetComponent<Rigidbody>();
    }
}
