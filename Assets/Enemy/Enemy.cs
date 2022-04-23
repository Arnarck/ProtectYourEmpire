using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject goldRewardText;
    [Tooltip("The effect that will be played when the enemy dies.")]
    [SerializeField] GameObject deathFX;
    [Tooltip("The amount of damage the enemy will do to the castle.")]
    [SerializeField] int damageToCastle = 1;
    [Tooltip("The amount of currencies the player will receive after destroying an enemy.")]
    [SerializeField] int goldReward = 10;

    Bank _bank;
    Castle _castle;
    BurnEffect _burnEffect;
    ShockEffect _shockEffect;
    FreezeEffect _freezeEffect;
    EnemySpawner _enemySpawner;
    PoolingSystem _deathFxPool;
    PoolingSystem _goldRewardTextPool;

    bool _isAlive;

    public bool IsAlive { get => _isAlive; }

    void Awake()
    {
        _bank = FindObjectOfType<Bank>();
        _castle = FindObjectOfType<Castle>();
        _burnEffect = GetComponent<BurnEffect>();
        _shockEffect = GetComponent<ShockEffect>();
        _freezeEffect = GetComponent<FreezeEffect>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    void OnEnable()
    {
        _isAlive = true;
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _deathFxPool = poolingSystemFinder.FindPoolingSystem(deathFX);
        _goldRewardTextPool = poolingSystemFinder.FindPoolingSystem(goldRewardText);
    }

    // Deactivates and resets all the already activated effects before activates the new one
    // Only one effect can be activated at a time
    // One effect can be charged up while other effect is active
    // But if an effect be completely charged while another one is running,
    // then the completely charged effect will be activated, and the effect that is already running, will be stopped
    public void SetupForBurnEffect()
    {
        _freezeEffect.StopEffectAbruptly();
        _shockEffect.StopEffectAbruptly();
    }

    public void SetupForShockEffect()
    {
        _burnEffect.StopEffectAbruptly();
        _freezeEffect.StopEffectAbruptly();
    }

    public void SetupForFreezingEffect()
    {
        _burnEffect.StopEffectAbruptly();
        _shockEffect.StopEffectAbruptly();
    }

    // Damages the player's castle when the enemy reaches its final destination (which will be the player's castle)
    public void DamagePlayerCastle()
    {
        _castle.TakeDamage(damageToCastle);
    }

    // Reward the player with gold after the player destroys the enemy
    public void RewardPlayer()
    {
        _bank.Deposit(goldReward);
    }

    // Deactivates the enemy after being destroyed by the player's towers, or after reaching the final destination
    public void DeactivateEnemy(bool wasKilled)
    {
        // Checks if the enemy is alive. Protects against two calls (EnemyCollision and EnemyMover) in the same frame
        if (!_isAlive) { return; }

        if (wasKilled)
        {
            PlayVFX(_deathFxPool);
            DisplayGoldReward(_goldRewardTextPool);
        }

        _isAlive = false;
        _enemySpawner.RecordEnemyDeath();
        gameObject.SetActive(false);
    }

    void PlayVFX(PoolingSystem vfxPool)
    {
        if (vfxPool == null) { return; }

        GameObject vfx = vfxPool.GetObject();

        vfx.transform.position = transform.position;
        vfx.transform.rotation = transform.rotation;
    }

    void DisplayGoldReward(PoolingSystem textPool)
    {
        if (textPool == null) { return; }

        GameObject tmpObject = textPool.GetObject();
        TextMeshPro tmp = tmpObject.GetComponent<TextMeshPro>();

        tmp.transform.position = transform.position;
        tmp.transform.rotation = transform.rotation;
        tmp.text = $"+${goldReward}";
    }
}
