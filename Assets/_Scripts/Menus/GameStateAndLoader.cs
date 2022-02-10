using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GamePersistence;

public class GameStateAndLoader : MonoBehaviour
{
    //Store the saved stats
    private SavedStats stats = new SavedStats();

    //Check if this script should load the next time the scene changes
    private bool _shouldLoadOnSceneChange = false;

    //The target loading scene's build index
    private int _sceneIndex;

    //Check if, after loading, the scene should be played from the beginning
    private bool _shouldStartFromBeginning;

    //The loading screen
    private GameObject _loadingScreen;

    private void Awake()
    {
        //Don't destroy this script on scene change
        DontDestroyOnLoad(gameObject);

        //Get the loading screen and set it to inactive
        _loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen");
        _loadingScreen.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //If this script should load a scene
        if (_shouldLoadOnSceneChange)
        {
            //Start loading
            StartCoroutine(LoadGameOnScene(_shouldStartFromBeginning));
        }
    }

    public void StartLoadingGame()
    {
        //Get the saved game's path
        string path = Application.persistentDataPath + "\\playerSave.dat";

        //If the file doesn't exist, end the function prematurely
        if (!File.Exists(path))
            return;

        //Open the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        //Store the deserialised saved stats
        stats = (SavedStats)bf.Deserialize(file);

        //Close the file to avoid read/write overlapping
        file.Close();

        //Set the target scene index as the one in the saved stats
        _sceneIndex = stats.Level;
        //Set the bool for starting a scene from the beginning or loading the player's progress
        _shouldStartFromBeginning = stats.ShouldStartFromBeginning;

        //Activate the loading indicator
        ActivateLoadingScreen();

        //Start the loading coroutine
        StartCoroutine(LoadGame(_sceneIndex, _shouldStartFromBeginning));
    }

    public void ActivateLoadingScreen()
    {
        //Activate the loading indicator
        _loadingScreen.SetActive(true);
    }

    public IEnumerator LoadGame(int level, bool shouldStartFromBeginning)
    {
        //Set the "scene load on start" bool
        _shouldLoadOnSceneChange = true;
        //Check if the scene loaded should start with the player's progress
        _shouldStartFromBeginning = shouldStartFromBeginning;

        //Wait a frame
        yield return new WaitForEndOfFrame();

        //Start asynchronously loading the target scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);
    }

    private IEnumerator LoadGameOnScene(bool shouldStartFromBeginning)
    {
        //Deactivate the loading indicator
        _loadingScreen.SetActive(false);

        //Load the game if the saving blocker didn't exist when the game was saved
        //If the saving blocker existed, only the level itself will be loaded
        if (!shouldStartFromBeginning)
        {
            //Wait a frame
            yield return new WaitForEndOfFrame();

            //Skip the tutorial (if there is any)
            GameManager.instance.DebugSkip();

            //Wait a frame
            yield return new WaitForEndOfFrame();

            //Load the saved game
            GameObject.FindGameObjectWithTag("GamePersistence").GetComponent<GamePersistence>().LoadGame();
        }

        //Reset the bool that checks if the script should load on scene change
        _shouldLoadOnSceneChange = false;
    }
}
