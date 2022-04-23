using System.Collections;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    [Tooltip("The effect that will be played when the elemental effect is applied.")]
    [SerializeField] GameObject burnFX;

    [Header("Effect Configuration Data")]
    [Tooltip("The damage dealt to the enemy each iteration of the Burn Effect.")]
    [SerializeField] int damage = 1;
    [Tooltip("The enemy's initial resistance to the effect.")]
    [SerializeField] int initialChargeLimit = 3;
    [Tooltip("The number of waves needed to increase the effect resistance.")]
    [SerializeField] int increaseRate = 3;
    [Tooltip("How much the enemy's resistance to the effect will increase.")]
    [SerializeField] int increaseAmount = 1;

    [Header("Timing")]
    [Tooltip("The number of times the enemy will receive damage.")]
    [SerializeField] int iterations = 10;
    [Tooltip("The time between iterations.")]
    [SerializeField] float burnTimer = .5f;

    Enemy _enemy;
    Coroutine _burn; // Stores the coroutine when it is activated. Why? To allow me to use StopCoroutine without strings
    AudioSource _burnSFX;
    WaveSystem _waveSystem;
    ParticleSystem _burnVFX;
    EnemyHealth _enemyCollision;


    bool _isBurning;
    int _chargeLimit;
    int _currentCharges;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _burnSFX = burnFX.GetComponent<AudioSource>();
        _waveSystem = FindObjectOfType<WaveSystem>();
        _burnVFX = burnFX.GetComponent<ParticleSystem>();
        _enemyCollision = GetComponent<EnemyHealth>();

        _chargeLimit = initialChargeLimit;
    }

    void OnEnable()
    {
        ResetStatus();
        IncreaseEffectResistance();
    }

    // Reset status to allow the effect to be applied again
    void ResetStatus()
    {
        SetElementalEffectActive(false);
        _burn = null;
        _isBurning = false;
        _currentCharges = 0;
    }

    // Plays / Stops the Elemental VFX & SFX
    void SetElementalEffectActive(bool isActive)
    {
        var emission = _burnVFX.emission;
        emission.enabled = isActive;

        if (isActive)
        {
            _burnSFX.Play();
        }
        else
        {
            _burnSFX.Stop();
        }
    }

    // Increases the enemy's effect resistance (in this case, fire) every a certain number of waves
    void IncreaseEffectResistance()
    {
        float rawIncreaseValue = _waveSystem.CurrentWave / increaseRate;
        int increaseValueThisWave = Mathf.FloorToInt(rawIncreaseValue);

        _chargeLimit = initialChargeLimit + increaseAmount * increaseValueThisWave;
    }

    // Increases the charge. Once the current charges reaches the limit, the effect will be applied
    public void ChargeUp()
    {
        // Checks if the effect is already active
        if (_isBurning) { return; }

        _currentCharges++;

        // Checks if the charges has reached the limit
        if (_currentCharges >= _chargeLimit)
        {
            _enemy.SetupForBurnEffect();
            ApplyBurnEffect();
        }
    }

    // Activates the effect
    void ApplyBurnEffect()
    {
        SetElementalEffectActive(true);
        _isBurning = true;
        _burn = StartCoroutine(Burn());
    }

    // Deals damage to enemy over time
    IEnumerator Burn()
    {
        // keeps dealing damage to enemy over time
        for (int i = 0; i < iterations; i++)
        {
            // "cool down" until deal damage to enemy again
            yield return new WaitForSeconds(burnTimer);
            _enemyCollision.ProcessHit(damage);
        }

        ResetStatus();
    }

    // Reset the states and stop the Burn coroutine (if is activated)
    // This method is called by the Enemy script when the enemy starts to freeze or taking shock
    public void StopEffectAbruptly()
    {
        // checks if the coroutine is being executed
        if (_burn != null)
        {
            StopCoroutine(_burn);
        }

        ResetStatus();
    }
}
