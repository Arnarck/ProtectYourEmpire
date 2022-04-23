using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInputResponse : MonoBehaviour
{
    [Tooltip("The Sound Effect that plays when the player clicks a button.")]
    [SerializeField] AudioClip buttonPressed;
    [Tooltip("The Sound Effect that plays when the player selects a tower.")]
    [SerializeField] AudioClip towerSelected;

    Castle _castle;
    PauseGame _pauseGame;
    AudioSource _audioSource;
    TowerSelector _towerSelector;
    TowerSelectorUI _towerSelectorUI;

    void Awake()
    {
        _castle = FindObjectOfType<Castle>();
        _pauseGame = FindObjectOfType<PauseGame>();
        _audioSource = GetComponent<AudioSource>();
        _towerSelector = FindObjectOfType<TowerSelector>();
        _towerSelectorUI = FindObjectOfType<TowerSelectorUI>();
    }

    // Load the first level.
    public void StartGame()
    {
        _audioSource.PlayOneShot(buttonPressed);
        LoadLevel(1);
    }

    // Activates the menu if it is deactivated, and vice versa.
    public void ToggleMenuState(GameObject menu)
    {
        menu.SetActive(!menu.activeInHierarchy);

        if (menu.activeInHierarchy)
        {
            _audioSource.PlayOneShot(buttonPressed);
        }
    }

    // Close the game.
    public void QuitGame()
    {
        _audioSource.PlayOneShot(buttonPressed);
        Application.Quit();
    }

    // Unpause the game.
    public void ContinueGame()
    {
        _pauseGame.TogglePauseMenu();
    }

    // Reload the current scene.
    public void RestartLevel()
    {
        _audioSource.PlayOneShot(buttonPressed);
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(currentLevelIndex);
    }

    // Load the Main Menu scene.
    public void BackToMenu()
    {
        _audioSource.PlayOneShot(buttonPressed);
        LoadLevel(0);
    }

    // Sets the tower selected by the player.
    public void SelectTower(Tower tower)
    {
        _audioSource.PlayOneShot(towerSelected);
        _towerSelector.SelectedTower = tower;
    }

    // Shows the tower currently selected by the player.
    public void DisplaySelectedTower(Sprite tower)
    {
        _towerSelectorUI.SelectedTowerDisplay = tower;
    }

    // Purchase a Health Restoration for the player's castle.
    public void RestoreCastleLife()
    {
        _audioSource.PlayOneShot(buttonPressed);
        _castle.PurchaseHealthRestoration();
    }

    // Load the next level of the game.
    public void LoadNextLevel()
    {
        int mainMenuIndex = SceneManager.GetSceneByName("Main Menu").buildIndex;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (nextLevelIndex > lastLevelIndex)
        {
            nextLevelIndex = mainMenuIndex;
        }

        LoadLevel(nextLevelIndex);
    }

    // Load a scene.
    void LoadLevel(int level)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level);
    }
}
