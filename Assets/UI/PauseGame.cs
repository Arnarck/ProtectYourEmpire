using System.Collections;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    [Header("FX")]
    [Tooltip("The effect that plays when the player completes the level.")]
    [SerializeField] GameObject levelCompleteFX;
    [Tooltip("The effect that plays when the player lose the game.")]
    [SerializeField] GameObject gameOverFX;

    [Header("Menus")]
    [Tooltip("The GameObject that contains the Pause Menu UI.")]
    [SerializeField] GameObject pauseMenu;
    [Tooltip("The GameObject that contains the Game Over UI.")]
    [SerializeField] GameObject gameOverMenu;
    [Tooltip("The GameObject that contains the Level Complete UI.")]
    [SerializeField] GameObject levelCompleteMenu;
    [Tooltip("The GameObject that contains the Tower Selector UI.")]
    [SerializeField] GameObject towerSelectorMenu;
    [Tooltip("The Sound Effect that plays when the player pause / unpause the game, or when the End Game Screen appears.")]
    [SerializeField] AudioClip buttonPressed;

    [Header("End Game Config.")]
    [Tooltip("How long the game will continue running after the end of the level.")]
    [SerializeField] float timerToEndGame = 1f;
    [Tooltip("The timescale the game will run when the level ends.")]
    [SerializeField] [Range(0f, 1f)] float endGameTimeScale = .5f;

    Castle _castle;
    AudioSource _audioSource;
    AudioManager _audioManager;
    TowerSelectorUI _towerSelectorUI;
    bool _isGamePaused;
    bool _isGameFinished;
    bool _hasPlayerWonTheGame;

    public bool IsGamePaused { get => _isGamePaused; }

    void Awake()
    {
        _castle = FindObjectOfType<Castle>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = FindObjectOfType<AudioManager>();
        _towerSelectorUI = FindObjectOfType<TowerSelectorUI>();
    }

    void Update()
    {
        if (!_isGameFinished)
        {
            RespondToPauseInput();
        }
    }

    // Checks if the player has pressed the "pause game" key
    void RespondToPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleTowerSelectorMenu();
        }
    }

    // Enables or disables the pause menu
    public void TogglePauseMenu()
    {
        // Doesn't allow pause menu to be toggled when "Tower Selector" menu is activated
        if (towerSelectorMenu.activeInHierarchy) { return; }

        pauseMenu.SetActive(!_isGamePaused);
        SetPauseState(!_isGamePaused);
    }

    // Change data to pause / resume the game flow
    void SetPauseState(bool isPaused)
    {
        _audioSource.PlayOneShot(buttonPressed);
        if (isPaused)
        {
            Time.timeScale = 0f;
            _isGamePaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            _isGamePaused = false;
        }

        _audioManager.SetVolumeState(_isGamePaused);
    }

    // Enables or disables the tower selector menu
    public void ToggleTowerSelectorMenu()
    {
        if (pauseMenu.activeInHierarchy) { return; }

        towerSelectorMenu.SetActive(!_isGamePaused);
        SetPauseState(!_isGamePaused);
        _towerSelectorUI.UpdateInformation();
    }

    // Set the data to end the level
    public void PrepareToFinishLevel(bool hasPlayerWon)
    {
        // Leaves this method if the game is already finished (protects agains double coroutine).
        if (_isGameFinished) { return; }

        _isGameFinished = true;
        _hasPlayerWonTheGame = hasPlayerWon;
        Time.timeScale = endGameTimeScale;

        // The game can be ended when the enemy reaches the castle entrance,
        // the enemy movement occurs in IEnumerator events (that executes after "Update"),
        // and the Pause Input occurs in Update.
        // Therefore, there is a possibility that the Pause or Tower Selector screen will be active when the game ends.
        pauseMenu.SetActive(false);
        towerSelectorMenu.SetActive(false);

        _castle.PlayEndOfLevelEffects(_hasPlayerWonTheGame);
        StartCoroutine(CountdownToFinishLevel());
    }

    // Wait a few seconds before calling the End Game screen and ending the level.
    IEnumerator CountdownToFinishLevel()
    {
        yield return new WaitForSeconds(timerToEndGame);
        ActivateEndGameScreen();
    }

    // Activates an EndGame screen based on who won (player or enemy)
    void ActivateEndGameScreen()
    {
        _audioSource.PlayOneShot(buttonPressed);
        SetPauseState(true);

        if (_hasPlayerWonTheGame)
        {
            levelCompleteMenu.SetActive(true);
        }
        else
        {
            gameOverMenu.SetActive(true);
        }
    }
}
