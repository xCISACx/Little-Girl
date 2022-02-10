using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBoltBehaviour : MonoBehaviour
{
    //Check if the rotation should be updated
    public bool ShouldUpdateRotation = true;

    public float
        //The target amount of rotation towards 180 degrees
        TargetAngleAmount = 1f,
        //The actual amount of rotation
        ActualAngleAmount = 1f;

    private void Update()
    {
        //If the rotation should be updated
        if (ShouldUpdateRotation)
        {
            //Lerp the actual amount of rotation to obtain a smooth transition
            ActualAngleAmount = Mathf.Lerp(ActualAngleAmount, TargetAngleAmount, Time.deltaTime);

            //Clamp the rotation amount
            ActualAngleAmount = Mathf.Clamp01(ActualAngleAmount);

            //Set the rotation as the actual amount towards 180 degrees
            transform.localEulerAngles = new Vector3(0f, 0f, ActualAngleAmount * 180f);
        }
    }
}
