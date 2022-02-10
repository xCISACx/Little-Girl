using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraAnalogueEffect : MonoBehaviour
{
    //The global post-processing profile for this level
    public PostProcessingProfile GlobalPostProcessingProfile;

    //The colour grading settings (used to change the gain)
    private ColorGradingModel.Settings _colorGrading;

    //The camera array
    public Object[] Cameras;

    //The limits between which the gain will float
    [SerializeField]
    private float _randomLimits = 0.03f;
    
    void Awake()
    {
        //Instance creation
        _colorGrading = GlobalPostProcessingProfile.colorGrading.settings;
        Cameras = Resources.FindObjectsOfTypeAll(typeof(Camera));
    }

    private void LateUpdate()
    {
        //Float the random index for the gain between the limits
        float randGain = Random.Range(-_randomLimits, _randomLimits);
        
        //Create an instance of a color grading settings profile
        _colorGrading = GlobalPostProcessingProfile.colorGrading.settings;

        //Edit the instance's values
        _colorGrading.colorWheels.linear.gain = new Color(1f, 1f, 1f, randGain);
        _colorGrading.colorWheels.linear.gamma = new Color(1f, 1f, 1f, randGain / 5f);

        //Set the post-processing profile's color grading as the edited one
        GlobalPostProcessingProfile.colorGrading.settings = _colorGrading;
    }
}
