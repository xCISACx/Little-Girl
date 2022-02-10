using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraSpecs : MonoBehaviour
{
    //The global post-processing profile for this level
    public PostProcessingProfile GlobalPostProcessingProfile;

    //The camera array
    public Object[] Cameras;

    //The static instance
    public static CameraSpecs instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        //Instance creation
        Cameras = Resources.FindObjectsOfTypeAll(typeof (Camera));

        foreach(Camera camera in Cameras)
        {
            //If this camera should be counted
            if (camera.CompareTag("MainCamera"))
            {
                //Add the post-processing behaviour and its profile to the camera
                camera.gameObject.AddComponent<PostProcessingBehaviour>();
                camera.gameObject.GetComponent<PostProcessingBehaviour>().profile = GlobalPostProcessingProfile;
            }
        }
    }
}
