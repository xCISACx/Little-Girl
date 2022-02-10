using System.Collections;
using UnityEngine;

public class ChangeCameraOnTriggerEnter : MonoBehaviour
{
    //The two cameras which will be exchanged
    public GameObject CameraActivate;
    
    private void Start()
    {        
        //Deactivate the mesh renderer to hide the actual box
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the camera changer on the main character comes into contact with this trigger
        if(other.CompareTag("CameraChanger"))
        {
            //If the target camera isn't already activated
            if (PlayerGlobalVariables.instance.CurrentCamera != CameraActivate.GetComponent<Camera>())
            {
                StartCoroutine(ChangeCamera());
            }
        }
    }

    public void StartCoroutineChangeCamera()
    {
        StartCoroutine(ChangeCamera());
    }

    IEnumerator ChangeCamera()
    {
        yield return null;

        //Deactivate the current camera
        PlayerGlobalVariables.instance.CurrentCamera.gameObject.SetActive(false);
        
        //Set the target camera as the current camera
        PlayerGlobalVariables.instance.CurrentCamera = CameraActivate.GetComponent<Camera>();
        
        //Activate the target camera
        CameraActivate.SetActive(true);
    }
}
