using System.Collections;
using TMPro;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Displayers")]
    [Tooltip("The visual representation of the current wave.")]
    [SerializeField] TextMeshProUGUI waveDisplay;
    [Tooltip("The visual representation of how much time is left until the next wave is started.")]
    [SerializeField] TextMeshProUGUI nextWaveTimer;

    [Header("Wave Configuration")]
    [Tooltip("The last wave of the level.")]
    [SerializeField][Range(1, 50)] int finalWave = 10;
    [Tooltip("The amount of currencies the player will receive for surviving a wave.")]
    [SerializeField] int waveReward = 10;
    [Tooltip("The time until the next wave starts.")]
    [SerializeField] int waveTimer = 5;

    Bank _bank;
    PauseGame _pauseGame;
    AudioSource _audioSource;
    EnemySpawner _enemySpawner;

    int _currentWave;
    bool _isFirstWave = true;
    bool _isWaveFinished = true;

    public int FinalWave { get => finalWave; }

    public int CurrentWave { get => _currentWave; }

    void Awake()
    {
        _bank = FindObjectOfType<Bank>();
        _audioSource = GetComponent<AudioSource>();
        _pauseGame = FindObjectOfType<PauseGame>();
        _enemySpawner = GetComponent<EnemySpawner>();        
    }

    void Start()
    {
        DisplayCurrentWave();
        StartCoroutine(NextWaveCountdown());
    }

    // Shows the current wave to the player.
    void DisplayCurrentWave()
    {
        string description = $"Wave: {_currentWave} / {finalWave}";
        waveDisplay.text = description;
    }

    // Wait a few seconds before starting the next wave.
    IEnumerator NextWaveCountdown()
    {
        nextWaveTimer.transform.parent.gameObject.SetActive(true);

        for (int i = waveTimer; i > 0 ; i--)
        {
            _audioSource.Play();
            nextWaveTimer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        nextWaveTimer.transform.parent.gameObject.SetActive(false);
        StartWave();
    }

    // Performs the necessary processes to start a new wave.
    void StartWave()
    {
        if (!_isFirstWave)
        {
            _bank.Deposit(waveReward);
            _enemySpawner.SetupNextWave();
        }

        _isWaveFinished = false;
        _currentWave++;
        DisplayCurrentWave();
        _enemySpawner.StartSpawning();
    }

    // Performs the necessary processes to end the current wave.
    public void FinishWave()
    {
        // Protects against a call of this method when the wave has already finished.
        if (_isWaveFinished) { return; }

        _isWaveFinished = true;
        _isFirstWave = false;

        // Checks if the current wave isn't the final wave.
        if (_currentWave < finalWave)
        {
            StartCoroutine(NextWaveCountdown());
        }
        else
        {
            _pauseGame.PrepareToFinishLevel(true);
        }
    }
}