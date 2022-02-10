using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CloseupCameraHandControls : MonoBehaviour
{
    Vector3 _handMovement;
    public Camera InteractionCamera;

    // Update is called once per frame
    void Update()
    {
        var horizontalInput = Input.GetAxis("Mouse X");
        var verticalInput = Input.GetAxis("Mouse Y");
        var screenHeight = Screen.height;
        var screenWidth = Screen.width;

        horizontalInput *= Time.deltaTime;
        verticalInput *= Time.deltaTime;

        var mousePosition = Input.mousePosition;
        
        var pos = InteractionCamera.ScreenToWorldPoint(mousePosition);
        var ray = InteractionCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            transform.position = hit.point;
        }
        
        //pos.x = Mathf.Clamp(pos.x, 0.07f, 0.93f);
        //pos.y = Mathf.Clamp(pos.y, 0.07f, 0.93f);
        
	    
       // var clampedHor = Mathf.Clamp(horizontalInput, 0, screenWidth);
        //var clampedVer = Mathf.Clamp(verticalInput, 0, screenHeight);
        
        //_handMovement = new Vector3(clampedHor, 0f, clampedVer);
        
        //transform.parent.position = pos;
        
        //Debug.Log(mousePosition);
        //Debug.Log(pos);
    }
}
