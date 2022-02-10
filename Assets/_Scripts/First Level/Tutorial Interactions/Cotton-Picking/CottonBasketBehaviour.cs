using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CottonBasketBehaviour : MonoBehaviour
{
    //The GameObjects in contact with this one
    public List<CottonPickUpBehaviour> InContact;

    public Vector3 DropPosition;
    
    public Vector3 WorldDropPosition => transform.position + DropPosition;

    public float DropRadius = 1;

    public LayerMask ObjectLayerMask;
    
    public bool LayerMaskContainsLayer(LayerMask mask, int layer)
    {
        
        return mask == (mask | (1 << layer));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(WorldDropPosition, 0.02f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, DropRadius);
    }
#endif

    protected void Start()
    {
        InContact = new List<CottonPickUpBehaviour>();
    }

    protected void OnTriggerStay(Collider other)
    {        
        if (LayerMaskContainsLayer(ObjectLayerMask, other.gameObject.layer))
        {
            //Debug.Log(other.GetComponent<Collider>().name);
            var cottonPickupBehaviour = other.GetComponent<CottonPickUpBehaviour>();
            //Add itself to the list of objects in contact with the plate
            
            if (!InContact.Contains(cottonPickupBehaviour))
            {
                InContact.Add(cottonPickupBehaviour);
            }
//            transform.SetParent(other.transform);
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (LayerMaskContainsLayer(ObjectLayerMask, other.gameObject.layer))
        {
            var cottonPickUpBehaviour = other.GetComponent<CottonPickUpBehaviour>();
            
            if (InContact.Contains(cottonPickUpBehaviour))
            {
                //Remove itself to the list of objects in contact with the plate
                InContact.Remove(cottonPickUpBehaviour);
            }
//            transform.SetParent(_parent);
        }
    }
}
