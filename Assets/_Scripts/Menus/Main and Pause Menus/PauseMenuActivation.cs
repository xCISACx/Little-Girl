using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuActivation : MonoBehaviour
{
    [SerializeField]
    //Used to check if the pause menu is currently active
    private bool _pauseMenuActive;

    public GameObject
        //The pause menu screen
        PauseMenu,
        //The options menu screen
        SettingsMenu,
        //The exit to menu confirmation screen
        ExitToMenuConfirmation,
        //The exit to desktop confirmation screen
        ExitToDesktopConfirmation;

    //The pause menu canvas
    private Canvas _pauseMenuCanvas;

    //The menu camera
    public GameObject
        MenuCamera,
        ReadableCamera;

    //Check if drawables should be drawn when the game continues
    public bool
        ShouldDrawOnContinue,
        ShouldDrawCursorOnContinue;

    private void Awake()
    {
        //Instance creation
        _pauseMenuCanvas = transform.parent.GetComponent<Canvas>();

        //Deactivate the menu camera
        MenuCamera.SetActive(false);

        //Reset the pause menu active check
        _pauseMenuActive = false;
    }

    private void OnScene()
    {
        //Deactivate the menu camera
        MenuCamera.SetActive(false);

        //Reset the pause menu active check
        _pauseMenuActive = false;

        //Reset the time scale
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        //If the pause menu flag isn't true
        if(!_pauseMenuActive)
        {
            //Activate the background and frame for the pause menu
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            //Check if the drawables were active when the pause menu was enabled
            ShouldDrawOnContinue = PlayerGlobalVariables.instance.ShouldDraw;
            ShouldDrawCursorOnContinue = PlayerGlobalVariables.instance.ShouldDrawCursor;
            //Draw mouse cursor and hide the drawables
            PlayerGlobalVariables.instance.ShouldDraw = false;
            PlayerGlobalVariables.instance.ShouldDrawCursor = true;

            //Deactivate all pause options except the main screen
            DeactivateAllExceptPause();

            //Stop time
            Time.timeScale = 0f;

            //Activate the pause menu canvas
            _pauseMenuCanvas.enabled = true;

            //Set the menu active flag
            _pauseMenuActive = true;
        }
        //If it is, check for the pause menu's state
        else
        {
            //Checks if the active menu is the pause menu
            if(PauseMenu.activeSelf)
            {
                ContinueGame();
            }
            //If not, take the user back to the pause menu
            else
            {
                DeactivateAllExceptPause();
            }
        }
    }

    public void ContinueGame()
    {
        //Disable the pause menu camera
        MenuCamera.SetActive(false);

        //Deactivate the pause menu canvas
        _pauseMenuCanvas.enabled = false;

        //Deactivate the pause menu overlay
        DeactivatePauseMenu();

        //Deactivate the pause menu's background and frame
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);

        //If the drawables shouldn't be drawn
        if(ShouldDrawOnContinue)
        {
            //Deactivate the drawables on exit
            PlayerGlobalVariables.instance.ShouldDraw = true;
        }

        //If the cursor shouldn't be drawn
        if(!ShouldDrawCursorOnContinue)
        {
            //Deactivate the cursor on exit
            PlayerGlobalVariables.instance.ShouldDrawCursor = false;
        }

        //Resume time
        Time.timeScale = 1f;

        //Reset the pause menu's flag
        _pauseMenuActive = false;

        if (!ReadableCamera.activeSelf)
        {
            PlayerGlobalVariables.instance.CanMove = true;
        }
    }

    private void DeactivateAllExceptPause()
    {
        //Deactivate all other screens
        DeactivateSettingsMenu();
        DeactivateExitToMenu();
        DeactivateExitToDesktop();

        //Activate the main pause menu screen
        ActivatePauseMenu();
    }

    #region Pause menu activation/deactivation
    public void ActivatePauseMenu()
    {
        //Activate the pause menu and its camera
        PauseMenu.SetActive(true);
        MenuCamera.SetActive(true);
        PlayerGlobalVariables.instance.CanMove = false;
    }

    public void DeactivatePauseMenu()
    {
        //Deactivate the pause menu
        PauseMenu.SetActive(false);

    }
    #endregion

    #region Settings menu activation/deactivation
    public void ActivateSettingsMenu()
    {
        //Deactivate the pause menu
        DeactivatePauseMenu();

        //Activate the settings menu
        SettingsMenu.SetActive(true);

        //Reset the settings options by reading saved options from file
        GetComponentInChildren<SettingsFileHandler>().ResetSettingsFromFile();
    }

    public void DeactivateSettingsMenu()
    {
        StartCoroutine(DeactivateSettings());
    }

    IEnumerator DeactivateSettings()
    {
        yield return new WaitForEndOfFrame();
        SettingsMenu.SetActive(false);
        ActivatePauseMenu();
    }
    #endregion

    #region Exit to menu activation/deactivation
    public void ActivateExitToMenu()
    {
        //Deactivate the pause menu
        DeactivatePauseMenu();

        //Activate the exit to menu confirmation
        ExitToMenuConfirmation.SetActive(true);
    }

    public void DeactivateExitToMenu()
    {
        //Deactivate the exit to menu confirmation
        ExitToMenuConfirmation.SetActive(false);

        //Activate the pause menu
        ActivatePauseMenu();
    }
    #endregion

    #region Exit to desktop activation/deactivation
    public void ActivateExitToDesktop()
    {
        //Deactivate the pause menu
        DeactivatePauseMenu();

        //Activate the exit to desktop confirmation
        ExitToDesktopConfirmation.SetActive(true);
    }

    public void DeactivateExitToDesktop()
    {
        //Deactivate the exit to desktop confirmation
        ExitToDesktopConfirmation.SetActive(false);

        //Activate the pause menu
        ActivatePauseMenu();
    }
    #endregion

    #region Exit commands
    public void ExitToMenu()
    {
        //Resume time
        Time.timeScale = 1f;

        //Load the main menu
        SceneManager.LoadScene(0);
    }

    public void ExitToDesktop()
    {
        //Resume time
        Time.timeScale = 1f;

        //Exit the game
        Application.Quit();
    }
    #endregion
}
