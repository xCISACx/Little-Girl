using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SettingsFileHandler : MonoBehaviour
{
    //The settings file handler
    private string _settingsFilePath;

    //The in-game settings handler
    private SettingsHandler _settingsHandler;

    void Awake()
    {
        //Setup the file folder path location
        _settingsFilePath = Application.persistentDataPath + "/settings";
        _settingsHandler = GetComponent<SettingsHandler>();

        //Reset the settings options by reading the ones saved in the file
        ResetSettingsFromFile();
    }

    private void Start()
    {
        #region Check for directory and file existence
        //If the file folder path doesn't exist, create it
        if (!Directory.Exists(_settingsFilePath))
        {
            Directory.CreateDirectory(_settingsFilePath);
        }
        //Check if the settings file exists and create it if it doesn't
        if (!File.Exists(_settingsFilePath + "/settings.cfg"))
        {
            SaveSettings(2, 2, 0.2f, 0f);
        }
        #endregion
    }

    public void ResetSettingsFromFile()
    {
        List<string> fileLines = new List<string>();

        //Check if the settings file exists
        if (File.Exists(_settingsFilePath + "/settings.cfg"))
        {
            //Read the settings lines from the file
            StreamReader settingsFile = new StreamReader(Application.persistentDataPath + "/settings/settings.cfg");
            string fileContents = settingsFile.ReadLine();

            //Pass the settings lines to an array and split the contents of the file
            string[] tempFileLines = new string[4];
            tempFileLines = fileContents.Split(';');

            //Add the lines to a list
            foreach (string s in tempFileLines)
            {
                fileLines.Add(s);
            }

            //Close the file
            settingsFile.Close();

            //Reset the settings with the loaded settings
            _settingsHandler.ResetAllSettings(fileLines);
        }
    }

    public void SaveSettings(int resIndex, int quaLevel, float briValue, float volLevel)
    {
        StartCoroutine(SaveSettingsToFile(resIndex, quaLevel, briValue, volLevel));
    }

    IEnumerator SaveSettingsToFile(int resIndex, int quaLevel, float briValue, float volLevel)
    {
        yield return new WaitForEndOfFrame();

        //Create a temporary list for the settings lines
        List<string> settingsContent = new List<string>();

        //Fill the temporary list with the settings specifications
        settingsContent.Add(resIndex.ToString() + ';');
        settingsContent.Add(quaLevel.ToString() + ';');
        settingsContent.Add(briValue.ToString() + ';');
        settingsContent.Add(volLevel.ToString() + ';');

        //Write all settings to the file
        File.WriteAllText(_settingsFilePath + "/settings.cfg", settingsContent[0] + settingsContent[1] + settingsContent[2] + settingsContent[3]);
    }
}
