using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScalePlateBehaviour : MonoBehaviour
{
    //The scale objects in contact with this plate
    public List<ScaleObject> InContact;

    //This plate's drop position
    public Vector3 DropPosition;

    //The radius in which objects can be dropped
    public float DropRadius = 1;
    
    //The drop position translated into world coordinates
    public Vector3 WorldDropPosition => transform.position + DropPosition;

    //The plate's mass
    public float Mass => InContact.Sum(x => x.Mass);

    //The target position
    public Vector3 TargetPosition;

    //The object's layer mask
    public LayerMask ObjectLayerMask;
    
    public bool LayerMaskContainsLayer(LayerMask mask, int layer)
    {
        //Return if the layer mask contains the layer specified
        return mask == (mask | (1 << layer));
    }

    protected void Start()
    {
        //Get a list of all objects in contact with this plate
        InContact = new List<ScaleObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //Lerp into the target position to create a smooth movement
        transform.localPosition = Vector3.Lerp(transform.localPosition, TargetPosition, Time.deltaTime);
    }

    protected void OnTriggerStay(Collider other)
    {        
        //If the layer mask contains the layer of the object colliding
        if (LayerMaskContainsLayer(ObjectLayerMask, other.gameObject.layer))
        {
            var scalePickupBehaviour = other.GetComponent<ScaleObject>();
            
            //If the "in-contact" list doesn't contain this object and the object isn't being held
            if (!InContact.Contains(scalePickupBehaviour) && !transform.parent.parent.GetComponent<ScaleMainBehaviour>().CurrentObject)
            {
                //Add itself to the list of objects in contact with the plate
                InContact.Add(scalePickupBehaviour);
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        //If the layer mask contains the layer of the object colliding
        if (LayerMaskContainsLayer(ObjectLayerMask, other.gameObject.layer))
        {
            var scalePickupBehaviour = other.GetComponent<ScaleObject>();
            
            //If the "in-contact" list contains this object
            if (InContact.Contains(scalePickupBehaviour))
            {
                //Remove itself to the list of objects in contact with the plate
                InContact.Remove(scalePickupBehaviour);
            }
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(WorldDropPosition, 0.02f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, DropRadius);
    }*/
}
