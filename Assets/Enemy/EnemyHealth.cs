using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("Moving Text")]
    [Tooltip("The Text that shows the damage dealt to the enemy.")]
    [SerializeField] GameObject movingText;
    [Tooltip("Randomize the position where the moving text will spawn.")]
    [SerializeField] float xRange = 2f, zRange = 2f;

    [Header("Health")]
    [Tooltip("The UI Element that represents the enemy Health Bar.")]
    [SerializeField] Slider healthBar;
    [Tooltip("The Text that represents the enemy Health Count.")]
    [SerializeField] TextMeshProUGUI healthCount;
    [Tooltip("The amount of hit points the enemy will start on first wave.")]
    [Range(1, 999)] [SerializeField] int startHitPoints = 30;

    [Header("Resistance Over Time")]
    [Tooltip("The amount of health that will be increased on each wave.")]
    [Range(0, 1000)] [SerializeField] int increaseAmount = 1;
    [Tooltip("How intense will be the damage the enemy will receive.")]
    [Range(0, 1000)] [SerializeField] int defaultDamageMultiplier = 1;

    Enemy _enemy;
    WaveSystem _waveSystem;
    PoolingSystem _movingTextPool;

    int _maxHitPoints;
    int _currentHitPoints;
    int _currentDamageMultiplier;

    public int DamageMultiplier
    {
        get => _currentDamageMultiplier;
        set => _currentDamageMultiplier = value;
    }

    // Get all the referencies and initialize the member variables.
    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _waveSystem = FindObjectOfType<WaveSystem>();

        _maxHitPoints = startHitPoints;
    }

    // Reset the enemy's states everytime he respawn.
    void OnEnable()
    {
        IncreaseHealth();

        _currentHitPoints = _maxHitPoints;
        _currentDamageMultiplier = defaultDamageMultiplier;

        healthBar.maxValue = _maxHitPoints;
        healthBar.value = _maxHitPoints;

        DisplayHealthCount();
    }

    // Increase the amount of health based on the current wave the enemy is being spawned.
    void IncreaseHealth()
    {
        _maxHitPoints = startHitPoints + increaseAmount * (_waveSystem.CurrentWave - 1);
    }

    // Displays the current amount of health.
    void DisplayHealthCount()
    {
        string description = $"{ _currentHitPoints } / { _maxHitPoints }";
        healthCount.text = description;
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _movingTextPool = poolingSystemFinder.FindPoolingSystem(movingText);
    }

    // Process the damage received.
    public void ProcessHit(int amount)
    {
        // Protects against negative numbers.
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        int hit = amount * _currentDamageMultiplier;
        _currentHitPoints -= hit;
        ActivateMovingText(hit);

        // Checks if the enemy is dead.
        if (_currentHitPoints < 1)
        {
            _currentHitPoints = 0;
            _enemy.RewardPlayer();
            _enemy.DeactivateEnemy(true);
        }

        healthBar.value = _currentHitPoints;
        DisplayHealthCount();
    }

    // Generates an moving text, showing how many hits the enemy has taken.
    void ActivateMovingText(int hit)
    {
        if (_movingTextPool == null) { return; }

        GameObject movingTextObject = _movingTextPool.GetObject();
        TextMeshPro movingText = movingTextObject.GetComponent<TextMeshPro>();

        movingText.text = hit.ToString();
        SetMovingTextCoordinates(movingText.transform);
    }

    // Sets a random position of the moving text when spawned.
    void SetMovingTextCoordinates(Transform damageText)
    {
        float xThrow = Random.Range(-xRange, xRange);
        float xOffset = transform.position.x + xThrow;

        float zThrow = Random.Range(-zRange, zRange);
        float zOffset = transform.position.z + zThrow;

        damageText.position = new Vector3(xOffset, transform.position.y, zOffset);
    }

    // Called by the "FreezeEffect" script when the effect ends.
    // When freezing, the enemy's damage multiplier increases.
    // When the effect ends, the multiplier is restored to default.
    public void ResetDamageMultiplier()
    {
        _currentDamageMultiplier = defaultDamageMultiplier;
    }
}
