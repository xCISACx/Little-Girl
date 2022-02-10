using System.Collections.Generic;
using UnityEngine;

public class CloseupCameraHandHeightCalculator : MonoBehaviour
{
    //The GameObject lists that should be considered to calculate height
    public List<GameObject>
        ScalePlates,
        ScaleObjects;

    //The target height when an object is hovered
    public Vector3 TargetHeight;

    //The hand's normal height
    private float _normalHeight;

    //Check if the hand's height was changed
    public bool ChangedHeight = false;

    //The object directly below the hand
    public GameObject Below;

    //The object selection script
    private CloseupCameraSelectObject _selectObjectScript;

    // Start is called before the first frame update
    void Start()
    {
        //Instance creation
        _selectObjectScript = GetComponent<CloseupCameraSelectObject>();
        _normalHeight = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Set the height change flag as false
        ChangedHeight = false;

        //If the hand is free or holding, it can move
        if (_selectObjectScript.HandState == HandStates.Free || _selectObjectScript.HandState == HandStates.Holding)
        {
            //Check each object and see if the hand is over it
            foreach (GameObject scaleObj in ScalePlates)
            {
                if (transform.position.x >= scaleObj.transform.position.x - 0.45f && transform.position.x <= scaleObj.transform.position.x + 0.45f)
                {
                    if (transform.position.z >= scaleObj.transform.position.z - 0.45f && transform.position.z <= scaleObj.transform.position.z + 0.45f)
                    {
                        //Correct hand height 
                        CorrectHeight(new Vector3(transform.position.x, scaleObj.transform.position.y + 0.2f, transform.position.z));

                        //Set the object below as the one hovered
                        Below = scaleObj;
                    }
                }
                else
                {
                    //If the height hasn't been changed
                    if (!ChangedHeight)
                    {
                        //Reset the hand's target height
                        TargetHeight = new Vector3(transform.position.x, _normalHeight, transform.position.z);

                        //Reset the object below
                        Below = null;
                    }
                }
            }

            //Check each object and see if the hand is over it
            foreach (GameObject obj in ScaleObjects)
            {
                if (transform.position.x >= obj.transform.position.x - 0.2f && transform.position.x <= obj.transform.position.x + 0.2f)
                {
                    if (transform.position.z >= obj.transform.position.z - 0.2f && transform.position.z <= obj.transform.position.z + 0.2f)
                    {
                        //Correct hand height
                        CorrectHeight(new Vector3(transform.position.x, obj.transform.position.y + 0.2f, transform.position.z));

                        //Set the object below as the one hovered
                        Below = obj;
                    }
                }
                else
                {
                    //If the height hasn't been changed
                    if (!ChangedHeight)
                    {
                        //Reset the hand's target height
                        TargetHeight = new Vector3(transform.position.x, _normalHeight, transform.position.z);

                        //Reset the object below
                        Below = null;
                    }
                }
            }
        }
        
        //Interpolate between the hand's current position and the target position
        transform.position = Vector3.Lerp(transform.position, TargetHeight, Time.deltaTime * 20f);
    }

    public void CorrectHeight(Vector3 TargetPosition)
    {
        //Set the height change flag as true
        ChangedHeight = true;

        //Change the target position
        TargetHeight = TargetPosition;
    }
}
