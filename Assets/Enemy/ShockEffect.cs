using System.Collections;
using UnityEngine;

public class ShockEffect : MonoBehaviour
{
    [Tooltip("The effect that will be played when the elemental effect is applied.")]
    [SerializeField] GameObject shockFX;

    [Header("Nerfs Due to Effect")]
    [Tooltip("How slowly the enemy will move after taking the shock.")]
    [SerializeField] [Range(0f, 1f)] float movementReducer = 0f;

    [Header("Effect Configuration Data")]
    [Tooltip("The enemy's initial resistance to the effect")]
    [SerializeField] int initialChargeLimit = 3;
    [Tooltip("The time the enemy will be taking shock")]
    [SerializeField] float shockTimer = 5f;
    [Tooltip("The number of waves needed to increase the effect resistance.")]
    [SerializeField] int increaseRate = 3;
    [Tooltip("How much the enemy's resistance to the effect will increase.")]
    [SerializeField] int increaseAmount = 1;

    Enemy _enemy;
    Coroutine _shock;
    AudioSource _shockSFX;
    EnemyMover _enemyMover;
    WaveSystem _waveSystem;
    ParticleSystem _shockVFX;

    bool _isTakingShock;
    int _chargeLimit;
    int _currentCharges;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _enemyMover = GetComponent<EnemyMover>();
        _shockSFX = shockFX.GetComponent<AudioSource>();
        _waveSystem = FindObjectOfType<WaveSystem>();
        _shockVFX = shockFX.GetComponent<ParticleSystem>();

        _chargeLimit = initialChargeLimit;
    }

    void OnEnable()
    {
        ResetComponentStatus();
        IncreaseEffectResistance();
    }

    void ResetComponentStatus()
    {
        SetElementalEffectActive(false);
        _shock = null;
        _currentCharges = 0;
        _isTakingShock = false;
    }

    void SetElementalEffectActive(bool isActive)
    {
        var emission = _shockVFX.emission;
        emission.enabled = isActive;

        if (isActive)
        {
            _shockSFX.Play();
        }
        else
        {
            _shockSFX.Stop();
        }
    }

    void IncreaseEffectResistance()
    {
        float rawIncreaseValue = _waveSystem.CurrentWave / increaseRate;
        int increaseValueBasedOnWave = Mathf.FloorToInt(rawIncreaseValue);

        _chargeLimit = initialChargeLimit + increaseAmount * increaseValueBasedOnWave;
    }

    public void ChargeUp()
    {
        if (_isTakingShock) { return; }

        _currentCharges++;

        if (_currentCharges >= _chargeLimit)
        {
            _enemy.SetupForShockEffect();
            ApplyShockEffect();
        }

    }

    void ApplyShockEffect()
    {
        SetElementalEffectActive(true);
        _isTakingShock = true;
        _enemyMover.MovementFactor = movementReducer;
        _shock = StartCoroutine(Shock());
    }

    IEnumerator Shock()
    {
        yield return new WaitForSeconds(shockTimer);
        ResetComponentStatus();
        ResetExternalStates();
    }

    void ResetExternalStates()
    {
        _enemyMover.ResetMovementFactor();
    }

    public void StopEffectAbruptly()
    {
        if (_shock != null)
        {
            StopCoroutine(_shock);
        }
        
        ResetComponentStatus();
        ResetExternalStates();
    }
}
