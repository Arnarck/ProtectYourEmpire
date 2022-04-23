using System.Collections;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    [Tooltip("The effect that will be played when the elemental effect is applied.")]
    [SerializeField] GameObject freezeFX;

    [Header("Nerfs Due to Effect")]
    [Tooltip("How much the damage done to the enemy will intensify after being frozen.")]
    [SerializeField] int damageMultiplier = 2;
    [Tooltip("How slowly the enemy will move after being frozen.")]
    [SerializeField] [Range(0f, 1f)] float movementReducer = .6f;

    [Header("Effect Configuration Data")]
    [Tooltip("The enemy's initial resistance to the effect")]
    [SerializeField] int initialChargeLimit = 3;
    [Tooltip("The time the enemy will be frozen")]
    [SerializeField] float freezeTimer = 5f;
    [Tooltip("The number of waves needed to increase the effect resistance.")]
    [SerializeField] int increaseRate = 3;
    [Tooltip("How much the enemy's resistance to the effect will increase.")]
    [SerializeField] int increaseAmount = 1;

    Enemy _enemy;
    Coroutine _freeze;
    WaveSystem _waveSystem;
    EnemyMover _enemyMover;
    AudioSource _freezeSFX;
    ParticleSystem _freezeVFX;
    EnemyHealth _enemyHealth;

    bool _isFreezing;
    int _chargeLimit;
    int _currentCharges;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _enemyMover = GetComponent<EnemyMover>();
        _freezeSFX = freezeFX.GetComponent<AudioSource>();
        _waveSystem = FindObjectOfType<WaveSystem>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _freezeVFX = freezeFX.GetComponent<ParticleSystem>();

        _chargeLimit = initialChargeLimit;
    }

    void OnEnable()
    {
        ResetComponentStatus();
        IncreaseEffectResistance();
    }

    // Reset the component status to allow the effect to be applied again
    void ResetComponentStatus()
    {
        SetElementalEffectActive(false);
        _freeze = null;
        _currentCharges = 0;
        _isFreezing = false;
    }

    void SetElementalEffectActive(bool isActive)
    {
        var emission = _freezeVFX.emission;
        emission.enabled = isActive;

        if (isActive)
        {
            _freezeSFX.Play();
        }
        else
        {
            _freezeSFX.Stop();
        }
    }

    void IncreaseEffectResistance()
    {
        float rawChargeLimitIncrement = _waveSystem.CurrentWave / increaseRate;
        int chargeLimitIncrement = Mathf.FloorToInt(rawChargeLimitIncrement);

        _chargeLimit = initialChargeLimit + increaseAmount * chargeLimitIncrement;
    }

    public void ChargeUp()
    {
        if (_isFreezing) { return; }

        _currentCharges++;

        if (_currentCharges >= _chargeLimit)
        {
            _enemy.SetupForFreezingEffect();
            ApplyFreezeEffect();
        }
    }

    void ApplyFreezeEffect()
    {
        SetElementalEffectActive(true);
        _isFreezing = true;
        _enemyMover.MovementFactor = movementReducer; // makes the enemy slower
        _enemyHealth.DamageMultiplier = damageMultiplier; // makes the enemy weaker
        _freeze = StartCoroutine(Freeze());
    }

    // Makes the enemy slower and weaker for a while
    IEnumerator Freeze()
    {
        yield return new WaitForSeconds(freezeTimer);
        ResetComponentStatus();
        ResetExternalStates();
    }

    // Reset the external component states right after the effect ends
    void ResetExternalStates()
    {
        _enemyMover.ResetMovementFactor();
        _enemyHealth.ResetDamageMultiplier();
    }

    public void StopEffectAbruptly()
    {
        if (_freeze != null)
        {
            StopCoroutine(_freeze);
        }

        ResetComponentStatus();
        ResetExternalStates();
    }
}
