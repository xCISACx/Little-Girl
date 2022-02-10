using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using System.Linq;

public class GamePersistence : MonoBehaviour
{
    //The stats that are saved
    public SavedStats stats = new SavedStats();

    //All cameras
    public List<CameraSerialization> Cameras;

    //The cameras' parent
    public GameObject CamerasParent;

    //The inventory's containers
    public List<ReadableMenuItemBehaviour> Containers;

    //The inventory's containers' parent
    public GameObject ContainersParent;

    //The level's puzzles
    public List<BasePuzzle> LevelPuzzles;

    //A dictionary to save each puzzle's state
    public Dictionary<string, bool> Puzzles;

    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }
    }
    #endif

    public void SaveGame()
    {
        //Get the saved game's path
        string path = Application.persistentDataPath + "\\playerSave.dat";

        //Initialise the binary formatter and create the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        //Set the version
        stats.Version = 1;

        //Save the level
        stats.Level = SceneManager.GetActiveScene().buildIndex;

        //Save a bool that states if the level should start from the beginning or load the player's progress
        stats.ShouldStartFromBeginning = GameObject.FindGameObjectWithTag("SavingBlocker") != null;
        
        //Get all level puzzles
        LevelPuzzles = FindObjectsOfType<BasePuzzle>().ToList();

        //Save each puzzle's state
        stats.SavedPuzzles = new Dictionary<string, bool>();
        foreach (var puzzle in LevelPuzzles)
        {
            stats.SavedPuzzles.Add(puzzle.ID, puzzle.SolutionFound);

            Debug.Log("SAVED PUZZLE WITH ID: " + puzzle.ID + " and state: " + puzzle.SolutionFound);
        }

        //Save all puzzle states and IDs
        stats.PuzzleStates = LevelPuzzles.Select(puzzle => puzzle.SolutionFound).ToList();
        stats.PuzzleIDs = LevelPuzzles.Select(puzzle => puzzle.ID).ToList();

        //Save the player's position
        stats.PlayerX = PlayerGlobalVariables.instance.transform.position.x;
        stats.PlayerY = PlayerGlobalVariables.instance.transform.position.y;
        stats.PlayerZ = PlayerGlobalVariables.instance.transform.position.z;

        //Save the player's rotation
        stats.PlayerRotX = PlayerGlobalVariables.instance.transform.eulerAngles.x;
        stats.PlayerRotY = PlayerGlobalVariables.instance.transform.eulerAngles.y;
        stats.PlayerRotZ = PlayerGlobalVariables.instance.transform.eulerAngles.z;

        //Save each individual collection's collected readables
        stats.LetterIDs = GameManager.instance.CollectedLetters.Select(letter => letter.Codename).ToList();
        stats.BookIDs = GameManager.instance.CollectedBooks.Select(book => book.Codename).ToList();
        stats.NewspaperIDs = GameManager.instance.CollectedNewspapers.Select(newspaper => newspaper.Codename).ToList();

        //Save the current camera's ID
        stats.CameraID = PlayerGlobalVariables.instance.CurrentCamera.GetComponent<CameraSerialization>().ID;

        //Serialize the file
        bf.Serialize(file, stats);

        Debug.Log("Saved...");

        //Close the file
        file.Close();
    }

    public void LoadGame()
    {
        //Get the saved game's path
        string path = Application.persistentDataPath + "\\playerSave.dat";

        //If there is no saved game file, end the function prematurely
        if (!File.Exists(path))
            return;

        //Initialise the binary formatter and open the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        
        //Deserialise the saved stats
        stats = (SavedStats) bf.Deserialize(file);

        
        foreach (var cam in FindObjectsOfType<CameraSerialization>())
        {
            Debug.Log(cam.ID);
        }

        //Store the initial camera (for later change to the saved camera)
        Camera previousCamera = PlayerGlobalVariables.instance.CurrentCamera;
        
        //Get all cameras
        Cameras = CamerasParent.GetComponentsInChildren<CameraSerialization>(true).ToList();
        
        //Get the camera to switch to
        var camera = Cameras.First(c => c.ID == stats.CameraID).GetComponent<Camera>();
        Debug.Log("switching to camera: " + camera.gameObject + " ID: " + camera.GetComponent<CameraSerialization>().ID);
        
        //Get all puzzles
        LevelPuzzles = FindObjectsOfType<BasePuzzle>().ToList();
        
        //Set all solved puzzles as solved
        foreach (var puzzle in stats.SavedPuzzles)
        {
            Debug.Log("FOUND SAVED PUZZLE WITH ID: " + puzzle.Key + " and state: " + puzzle.Value);

            var puzzleToSet = LevelPuzzles.Find(p => p.ID == puzzle.Key);
            puzzleToSet.SolutionFound = puzzle.Value;
            if (puzzleToSet.SolutionFound)
            {
                puzzleToSet.Solve();
            }
        }

        //Temporarily disable the NavMeshAgent's transform update
        PointAndClickBehaviour.instance._navMeshAgent.updatePosition = false;
        PointAndClickBehaviour.instance._navMeshAgent.updateRotation = false;
        PointAndClickBehaviour.instance._navMeshAgent.enabled = false;
        
        //Get the player's transform and set its position and rotation to the ones saved
        var playerTransform = PlayerGlobalVariables.instance.transform;
        playerTransform.position = new Vector3(stats.PlayerX, stats.PlayerY, stats.PlayerZ);
        playerTransform.localEulerAngles = new Vector3(stats.PlayerRotX, stats.PlayerRotY, stats.PlayerRotZ);
        
        //Reset the agent's destination
        PointAndClickBehaviour.instance._navMeshAgent.nextPosition = playerTransform.position;
        PointAndClickBehaviour.instance.destinationPos = playerTransform.position;

        //Reenable the NavMeshAgent's transform update
        PointAndClickBehaviour.instance._navMeshAgent.updatePosition = true;
        PointAndClickBehaviour.instance._navMeshAgent.updateRotation = true;
        PointAndClickBehaviour.instance._navMeshAgent.enabled = true;
        
        //Deactivate the initial camera
        previousCamera.gameObject.SetActive(false);

        //Set the current camera as the one saved
        PlayerGlobalVariables.instance.CurrentCamera = camera;

        //Activate the saved camera
        PlayerGlobalVariables.instance.CurrentCamera.gameObject.SetActive(true);

        //Get the inventory containers
        Containers = ContainersParent.GetComponentsInChildren<ReadableMenuItemBehaviour>(true).ToList();
        
        //Reset all containers
        foreach (var container in Containers)
        {
            Destroy(container.gameObject);
        }
        
        //Get all readables
        var readableProperties = FindObjectsOfType<ReadableProperties>().ToList();

        //Reset all readable collections
        GameManager.instance.CollectedReadables.Clear();
        GameManager.instance.CollectedLetters.Clear();
        GameManager.instance.CollectedBooks.Clear();
        GameManager.instance.CollectedNewspapers.Clear();

        #region Fill each readable collection with the saved collected readables
        foreach (var ID in stats.LetterIDs)
        {
            var letter = readableProperties.Find(l => l.Codename == ID);
            if (!letter)
            {
                continue;
            }
            letter.GetComponentInChildren<ReadableObjectBehaviour>().Collect(letter);
        }
        
        foreach (var ID in stats.BookIDs)
        {
            var book = readableProperties.Find(l => l.Codename == ID);
            if (!book)
            {
                continue;
            }
            book.GetComponentInChildren<ReadableObjectBehaviour>().Collect(book);
        }
        
        foreach (var ID in stats.NewspaperIDs)
        {
            var news = readableProperties.Find(l => l.Codename == ID);
            if (!news)
            {
                continue;
            }
            news.GetComponentInChildren<ReadableObjectBehaviour>().Collect(news);
        }
        #endregion

        Debug.Log("Loaded...");

        //Close the file
        file.Close();
    }
    
    [Serializable]
    public struct SavedStats
    {
        public int Version;
        
        public int Level;

        public bool ShouldStartFromBeginning;
        
        public List<string> PuzzleIDs;
        public List<bool> PuzzleStates;

        public Dictionary<string, bool> SavedPuzzles;

        public float PlayerX;
        public float PlayerY;
        public float PlayerZ;
        
        public float PlayerRotX;
        public float PlayerRotY;
        public float PlayerRotZ;
        
        public List<string> LetterIDs;
        public List<string> BookIDs;
        public List<string> NewspaperIDs;

        public string CameraID;
    }
}