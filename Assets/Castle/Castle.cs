using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [Header("The UI Element that represents the castle Life Bar.")]
    [SerializeField] Slider lifeBar;

    [Header("FX")]
    [Tooltip("The effect that is played when the castle is healed.")]
    [SerializeField] GameObject healFX;
    [Tooltip("The effect that is played when the castle is at half HP or less.")]
    [SerializeField] GameObject damageFX;
    [Tooltip("The effect that is played when the castle takes damage.")]
    [SerializeField] GameObject hitFX;
    [Tooltip("The effect that is played when the player completes the level.")]
    [SerializeField] GameObject levelCompleteFX;
    [Tooltip("The effect that is played when the player loses the game.")]
    [SerializeField] GameObject gameOverFX;

    [Header("Castle Data")]
    [Header("The amount of health the castle starts the game with.")]
    [SerializeField] int startHealth = 5;
    [Header("The amount of health restored when the player draws a Health Restoration.")]
    [SerializeField] int restoreAmount = 1;
    [Header("The price to restore the Castle's Health.")]
    [SerializeField] int restorationCost = 10;

    int _currentHealth;
    int _maxHealth;

    Bank _bank;
    PauseGame _pauseGame;
    PoolingSystem _hitFxPool, _healFxPool;

    public int RestorationCost { get => restorationCost; }

    void Awake()
    {
        _bank = FindObjectOfType<Bank>();
        _pauseGame = FindObjectOfType<PauseGame>();
        _currentHealth = startHealth;
        _maxHealth = startHealth;
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _hitFxPool = poolingSystemFinder.FindPoolingSystem(hitFX);
        _healFxPool = poolingSystemFinder.FindPoolingSystem(healFX);

        lifeBar.maxValue = startHealth;
        lifeBar.value = startHealth;
    }

    // Called from the "TowerSelectorUI" script.
    // Allows to restore the Castle health by paying enough currencies
    public void PurchaseHealthRestoration()
    {
        // Cancel the purchase if the castle health is already full
        if (_currentHealth >= _maxHealth) { return; }

        bool wasTransactionSuccessful = _bank.Withdraw(restorationCost);

        if (wasTransactionSuccessful)
        {
            RestoreHealth();
        }
    }

    // Restore a certain amount of health
    void RestoreHealth()
    {
        if (restoreAmount < 0)
        {
            restoreAmount = Mathf.Abs(restoreAmount);
        }

        PlayVFX(_healFxPool);
        _currentHealth += restoreAmount;

        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }

        ToggleDamageFX();
        lifeBar.value = _currentHealth;
    }

    // Gets a VFX instance from its pool, and plays it in the castles coordinates.
    void PlayVFX(PoolingSystem vfxPool)
    {
        if (vfxPool == null) { return; }

        GameObject vfx = vfxPool.GetObject();

        vfx.transform.position = transform.position;
        vfx.transform.rotation = transform.rotation;
    }

    // Enables or disables the Damage FX based on the current health
    void ToggleDamageFX()
    {
        int halfHealth = _maxHealth / 2;
        bool canActivateDamageFX = false;

        if (_currentHealth <= halfHealth)
        {
            canActivateDamageFX = true;
        }

        damageFX.SetActive(canActivateDamageFX);
    }

    // Called from the "EnemyMover" script when the enemy reaches the last waypoint (the castle's entrance)
    // Makes the castle lose health and ends the game when the castle's health reaches 0 or less
    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            amount = Mathf.Abs(amount);
        }

        PlayVFX(_hitFxPool);
        _currentHealth -= amount;

        if (_currentHealth < 1)
        {
            _currentHealth = 0;
            _pauseGame.PrepareToFinishLevel(false);
        }

        ToggleDamageFX();
        lifeBar.value = _currentHealth;
    }

    // Plays an effect based on the argument value when the game is over
    public void PlayEndOfLevelEffects(bool hasPlayerWon)
    {
        if (hasPlayerWon)
        {
            levelCompleteFX.SetActive(true);
        }
        else
        {
            gameOverFX.SetActive(true);
        }
    }
}
