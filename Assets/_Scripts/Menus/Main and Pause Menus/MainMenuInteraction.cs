using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuInteraction : MonoBehaviour
{
    //The continue button
    public Button ContinueButton;

    //The new game confirmation screen
    public GameObject NewGameConfirmationScreen;

    //The continue button's colours
    public Color
        ActiveButtonNormal,
        ActiveButtonHighlight,
        InactiveButtonNormal,
        InactiveButtonHighlight;

    public Canvas
        //The main menu canvas
        MainCanvas,
        //The options menu canvas
        OptionsCanvas,
        //The exit confirmation canvas
        ExitConfirmationCanvas;

    private GraphicRaycaster
        //The main menu raycast detector
        MCGraphicRaycaster,
        //The options menu raycast detector
        OCGraphicRaycaster,
        //The exit confirmation raycast detector
        ECGraphicRaycaster;

    //The main camera's animator
    private Animator _animator;

    //The "game was already running" marker
    public GameObject GameWasAlreadyRunning;

    public GameObject
        //The main menu's panel
        MainMenuPanel,
        //The credits screen panel
        CreditsPanel;

    private void Start()
    {
        string path = Application.persistentDataPath + "\\playerSave.dat";
        OnMouseOverTextColour continueButtonColours = ContinueButton.GetComponent<OnMouseOverTextColour>();
        //If the save file exists, activate the continue button and change its colours to active
        if(File.Exists(path))
        {
            ContinueButton.interactable = true;
            continueButtonColours._NormalColour = ActiveButtonNormal;
            ContinueButton.GetComponentInChildren<Text>().color = ActiveButtonNormal;
            continueButtonColours._HightlightedColour = ActiveButtonHighlight;
        }
        //If not, deactivate the button and change its colours to inactive
        else
        {
            ContinueButton.interactable = false;
            continueButtonColours._NormalColour = InactiveButtonNormal;
            ContinueButton.GetComponentInChildren<Text>().color = InactiveButtonNormal;
            continueButtonColours._HightlightedColour = InactiveButtonHighlight;
        }

        Cursor.visible = true;

        //Instance creation
        _animator = GetComponent<Animator>();
        MCGraphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        OCGraphicRaycaster = OptionsCanvas.GetComponent<GraphicRaycaster>();
        ECGraphicRaycaster = ExitConfirmationCanvas.GetComponent<GraphicRaycaster>();

        //Deactivate all menus
        MCGraphicRaycaster.enabled = false;
        OCGraphicRaycaster.enabled = false;
        ECGraphicRaycaster.enabled = false;

        //Check if the game was already running
        if(GameObject.FindGameObjectWithTag("GameLoader"))
        {
            //Skip the introductory "press any key to continue"
            StartGameFromMenu();
        }
        //If the game wasn't already running
        else
        {
            //Play the introductory "press any key to continue"
            StartGameFromScratch();
        }
    }

    #region Handle travel from the main menu to the options screen
    public void ReturnToMainMenuFromOptions()
    {
        StartCoroutine("TravelMainMenuFromOptions");
    }

    private IEnumerator TravelMainMenuFromOptions()
    {
        //Reset all the graphic raycasters
        SetAllRaycastersFalse();

        //Reset all the animator's bools
        SetAllAnimatorBoolsFalse();
        //Set this option's bool to true
        _animator.SetBool("FromOptionsToMain", true);

        yield return new WaitForSecondsRealtime(2f);

        //Enable this option's graphic raycaster
        MCGraphicRaycaster.enabled = true;
    }
    #endregion

    #region Handle travel from the main menu to the exit
    public void ReturnToMainMenuFromExit()
    {
        StartCoroutine("TravelMainMenuFromExit");
    }

    private IEnumerator TravelMainMenuFromExit()
    {
        //Reset all the graphic raycasters
        SetAllRaycastersFalse();

        //Reset all the animator's bools
        SetAllAnimatorBoolsFalse();
        //Set this option's bool to true
        _animator.SetBool("FromExitToMain", true);

        yield return new WaitForSecondsRealtime(2f);

        //Enable this option's graphic raycaster
        MCGraphicRaycaster.enabled = true;
    }
    #endregion

    #region Handle travel from the options menu to the main menu
    public void OpenOptions()
    {
        //_settings.ResetOptions();
        StartCoroutine("TravelOptions");
    }

    private IEnumerator TravelOptions()
    {
        OptionsCanvas.GetComponentInChildren<SettingsFileHandler>().ResetSettingsFromFile();

        //Reset all the graphic raycasters
        SetAllRaycastersFalse();

        //Reset all the animator's bools
        SetAllAnimatorBoolsFalse();
        //Set this option's bool to true
        _animator.SetBool("FromMainToOptions", true);

        yield return new WaitForSecondsRealtime(2f);

        //Enable this option's graphic raycaster
        OCGraphicRaycaster.enabled = true;
    }
    #endregion

    #region Handle travel from the exit to the main menu
    public void ExitGameConfirmation()
    {
        StartCoroutine("TravelExitConfirmation");
    }

    private IEnumerator TravelExitConfirmation()
    {
        //Reset all the graphic raycasters
        SetAllRaycastersFalse();

        //Reset all the animator's bools
        SetAllAnimatorBoolsFalse();
        //Set this option's bool to true
        _animator.SetBool("FromMainToExit", true);

        yield return new WaitForSecondsRealtime(2f);

        //Enable this option's graphic raycaster
        ECGraphicRaycaster.enabled = true;
    }
    #endregion

    #region Reset all raycasters and animator bools
    private void SetAllRaycastersFalse()
    {
        MCGraphicRaycaster.enabled = false;
        OCGraphicRaycaster.enabled = false;
        ECGraphicRaycaster.enabled = false;
    }

    private void SetAllAnimatorBoolsFalse()
    {
        _animator.SetBool("FromOptionsToMain", false);
        _animator.SetBool("FromMainToOptions", false);
        _animator.SetBool("FromExitToMain", false);
        _animator.SetBool("FromMainToExit", false);
    }
    #endregion

    private void StartGameFromMenu()
    {
        //Reenable the main menu's interactivity
        MCGraphicRaycaster.enabled = true;

        //Set the camera's position next to the main menu
        _animator.Play("StayMainMenu");
    }

    private void StartGameFromScratch()
    {
        StartCoroutine(CheckPlayerInput());

        //Mark the game as launched to avoid replaying this bit next time
        Instantiate(GameWasAlreadyRunning);
    }

    private IEnumerator CheckPlayerInput()
    {
        //Wait for the player's input
        yield return new WaitUntil(() => Input.anyKey);

        //Play the transition animation
        _animator.SetBool("PlayerHasPressed", true);

        //Wait until the transition is finished
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsName("StayMainMenu"));

        //Reenable the main menu's interactivity
        MCGraphicRaycaster.enabled = true;
    }

    public void ShowCredits()
    {
        StartCoroutine(ShowCreditsScreen());
    }

    private IEnumerator ShowCreditsScreen()
    {
        //Deactivate the main menu's panel
        MainMenuPanel.SetActive(false);

        //Wait
        yield return new WaitForSeconds(0.5f);

        //Activate the credits panel
        CreditsPanel.SetActive(true);
    }

    public void GoBackFromCredits()
    {
        StartCoroutine(GoBackFromCreditsToMain());
    }

    private IEnumerator GoBackFromCreditsToMain()
    {
        //Deactivate the credits panel
        CreditsPanel.SetActive(false);

        //Wait
        yield return new WaitForSeconds(0.5f);

        //Activate the main menu's panel
        MainMenuPanel.SetActive(true);
    }

    public void CheckConfirmNewGame()
    {
        //If the continue button is interactable, there is an existing saved game
        if(ContinueButton.interactable)
        {
            NewGameConfirmationScreen.SetActive(true);
        }
        else
        {
            NewGame();
        }
    }

    public void NewGame()
    {
        //Starts a new game
        //SceneManager.LoadScene(1);

        //Loads the first level from the beginning
        GameObject.FindGameObjectWithTag("GameLoader").GetComponent<GameStateAndLoader>().ActivateLoadingScreen();
        StartCoroutine(GameObject.FindGameObjectWithTag("GameLoader").GetComponent<GameStateAndLoader>().LoadGame(1, true));
    }

    public void LoadGame()
    {
        //Starts loading the saved game level
        GameObject.FindGameObjectWithTag("GameLoader").GetComponent<GameStateAndLoader>().StartLoadingGame();
    }

    public void ExitGame()
    {
        //Quits the application
        Application.Quit();
    }
}
