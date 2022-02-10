using UnityEngine;

public class ObjectHandPosition : MonoBehaviour
{
    //The hand's Transform component
    private Transform _hand;

    //The holding offset for the object
    public Vector3 HoldingOffset;

    // Start is called before the first frame update
    void Start()
    {
        //Instance creation
        _hand = GameObject.FindGameObjectWithTag("Hand").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Reset the position according to the holding offset relative to the hand's position
        transform.position = _hand.position + HoldingOffset;
    }
}
