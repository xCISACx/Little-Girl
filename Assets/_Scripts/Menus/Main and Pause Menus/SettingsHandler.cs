using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

public class SettingsHandler : MonoBehaviour
{
    //The master audio mixer
    public AudioMixer MasterAudioMixer;

    //The value used to correct gamma (for brightness setting)
    public float GammaCorrection;

    //The settings file handler
    private SettingsFileHandler _settingsFileHandler;

    //The variable that holds the quality level
    private int _qualityLevel;

    //The variable that holds the brightness value
    private float _brightnessValue;

    //The variable that holds the volume value
    private float _volumeValue;

    //The available resolutions
    private Resolution[] _resolutions
                                      =
                                        {
                                            new Resolution(),
                                            new Resolution(),
                                            new Resolution(),
                                        };

    //The current resolution index
    private int _resIndex = 2;
    
    //The settings options
    public Dropdown
        ResolutionDropdown,
        QualityDropdown;
    public Slider
        BrightnessSlider,
        VolumeSlider;
    
    private void Awake()
    {
        //Set the resolution values
        ResetDefaultResolutionValues();

        //Instance creation
        _settingsFileHandler = GetComponent<SettingsFileHandler>();
    }

    private void ResetDefaultResolutionValues()
    {
        _resolutions[0].width = 1280;
        _resolutions[0].height = 720;

        _resolutions[1].width = 1366;
        _resolutions[1].height = 768;

        _resolutions[2].width = 1920;
        _resolutions[2].height = 1080;
    }

    #region Change settings
    public void SetResolution(Dropdown resolution)
    {
        //Get the resolution option
        _resIndex = resolution.value;
    }

    public void SetQualityLevel(Dropdown quality)
    {
        //Set the quality level to the user input
        QualitySettings.SetQualityLevel(quality.value);
        _qualityLevel = quality.value;
    }

    public void SetBrightness(Slider brightness)
    {
        //Adjust the brightness value to the colour number scheme
        GammaCorrection = brightness.value * 0.4f;

        //Change the ambient light to apply the brightness correction
        RenderSettings.ambientLight = new Color(GammaCorrection, GammaCorrection, GammaCorrection, 1.0f);
        _brightnessValue = GammaCorrection;
    }

    public void SetVolume(Slider volume)
    {
        //The volume value adjusted for decibels
        float adjVolume = volume.value;
        adjVolume *= 60f;
        adjVolume -= 48f;

        //Change the mixer float to match the user input
        MasterAudioMixer.SetFloat("MasterVolume", adjVolume);
        _volumeValue = adjVolume;
    }
    #endregion

    public void ConfirmOptions()
    {
        //Save the settings SaveSettingsto the file
        _settingsFileHandler.SaveSettings(_resIndex, _qualityLevel, _brightnessValue, _volumeValue);

        //Set the resolution and quality level settings
        Screen.SetResolution(_resolutions[_resIndex].width, _resolutions[_resIndex].height, true);
        QualitySettings.SetQualityLevel(_qualityLevel);

        /*/Reset all interaction button and cursor sizes
        foreach(ActionDrawable action in PlayerGlobalVariables.instance.AllActionDrawables)
        {
            action.DefineButtonSize();
        }
        GameManager.instance.DefineCursorSize();*/

        //Check if this is the main menu scene
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //Return to the main menu
            Camera.main.GetComponent<MainMenuInteraction>().ReturnToMainMenuFromOptions();
        }
        else
        {
            //Reactivate the pause menu and deactivate the settings menu
            transform.parent.parent.GetComponent<PauseMenuActivation>().ActivatePauseMenu();
            transform.parent.parent.GetComponent<PauseMenuActivation>().DeactivateSettingsMenu();
        }
    }

    public void ResetAllSettings(List<string> fileLines)
    {
        //Reset all values according to the lines received
        _resIndex = Int32.Parse(fileLines[0]);
        _qualityLevel = Int32.Parse(fileLines[1]);
        _brightnessValue = float.Parse(fileLines[2]);
        _volumeValue = float.Parse(fileLines[3]);

        //Reset resolution and quality settings
        Screen.SetResolution(_resolutions[_resIndex].width, _resolutions[_resIndex].height, true);
        QualitySettings.SetQualityLevel(_qualityLevel);

        //Reset the menu dropdowns and sliders to match the settings loaded
        UpdateSettingsValues();
    }

    private void UpdateSettingsValues()
    {
        //Update the resolution value with the one loaded from the text file
        ResolutionDropdown.value = _resIndex;

        //Update the quality value with the one loaded from the text file
        QualityDropdown.value = _qualityLevel;

        //Update the brightness correction, translating it into slider values
        float gammaCorrection = _brightnessValue / 0.4f;
        BrightnessSlider.value = gammaCorrection;

        //Update the volume value, translating it into slider values
        float adjVolume = _volumeValue;
        adjVolume += 48f;
        adjVolume /= 60f;
        VolumeSlider.value = adjVolume;
    }
}
