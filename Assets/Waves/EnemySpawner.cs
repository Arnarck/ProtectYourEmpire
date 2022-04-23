using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("The Enemy's GameObject.")]
    [SerializeField] GameObject enemyPrefab;
    [Tooltip("The  UI representation of how many enemies remain on the current wave.")]
    [SerializeField] TextMeshProUGUI remainingEnemiesDisplay;
    [Header("Spawn Configuration")]
    [Tooltip("The time interval before spawning another enemy.")]
    [SerializeField] int spawnTimer = 1;
    [Tooltip("The number of enemies that will be spawned in the first wave.")]
    [Range(1, 100)] [SerializeField] int startEnemies = 3;
    [Tooltip("The number of enemies that will be added to each wave.")]
    [Range(1, 100)][SerializeField] int difficultyRamp = 2;

    GameObject[] _pool;
    WaveSystem _waveSystem;

    int _totalEnemiesThisWave;
    int _enemiesRemainingThisWave;

    void Awake()
    {
        _waveSystem = GetComponent<WaveSystem>();

        _totalEnemiesThisWave = startEnemies;
        _enemiesRemainingThisWave = _totalEnemiesThisWave;
    }

    void Start()
    {
        PopulatePool();
    }

    // Fill the enemies pool, so the script don't need to instantiate more during the game.
    void PopulatePool()
    {
        // Calculation to predict the total number of enemies that will be instantiated
        int totalEnemies = startEnemies + difficultyRamp * (_waveSystem.FinalWave - 1);
        _pool = new GameObject[totalEnemies];

        for (int i = 0; i < _pool.Length; i++)
        {
            _pool[i] = Instantiate(enemyPrefab);
            _pool[i].transform.SetParent(transform);
            _pool[i].SetActive(false);
        }
    }

    // Called from WaveSystem to spawn the enemies for the first wave.
    public void StartSpawning()
    {
        DisplayEnemiesRemaining();
        StartCoroutine(SpawnEnemy());
    }

    // Shows the updated amount of living enemies left.
    void DisplayEnemiesRemaining()
    {
        string description = $"Enemies Remaining: {_enemiesRemainingThisWave} / {_totalEnemiesThisWave}";
        remainingEnemiesDisplay.text = description;
    }

    // Spawns enemies through time
    IEnumerator SpawnEnemy()
    {
        int enemiesSpawnedThisWave = 0;

        // restricts the number of enemies spawned per wave
        while (enemiesSpawnedThisWave < _totalEnemiesThisWave)
        {
            bool hasActivatedAnEnemy = ActivateEnemy();

            if (hasActivatedAnEnemy)
            {
                enemiesSpawnedThisWave++;
                // "cool down" after spawning an enemy
                yield return new WaitForSeconds(spawnTimer);
            }
            else
            {
                // Keep the game running, and return to this method in the next frame
                yield return new WaitForEndOfFrame();
            }
        }
    }

    // Activates the first deactivated enemy found
    bool ActivateEnemy()
    {
        if (_pool == null) { return false; }

        foreach (GameObject enemy in _pool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return true;
            }
        }

        return false;
    }

    // Counts the destroyed enemies in the current wave. After everyone be destroyed, finishes the wave.
    public void RecordEnemyDeath()
    {
        _enemiesRemainingThisWave--;
        DisplayEnemiesRemaining();

        // Checks if all enemies of the current wave has been destroyed
        if (_enemiesRemainingThisWave < 1)
        {
            _waveSystem.FinishWave();
        }
    }

    // Updates the data for the next wave.
    public void SetupNextWave()
    {
        _totalEnemiesThisWave += difficultyRamp;
        _enemiesRemainingThisWave = _totalEnemiesThisWave;
    }
}
