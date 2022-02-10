using UnityEngine;

public class HandPosition : MonoBehaviour
{
    //The hand's Transform component
    private Transform _hand;

    // Start is called before the first frame update
    void Start()
    {
        //Instance creation
        _hand = GameObject.FindGameObjectWithTag("Hand").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Set the xx and zz coordinates of this object as the hand's whilst maintaining its yy coordinate
        transform.position = new Vector3(_hand.position.x, transform.position.y, _hand.position.z);
    }
}
