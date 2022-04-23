using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    Enemy _enemy;
    BurnEffect _burnEffect;
    EnemyHealth _enemyHealth;
    ShockEffect _shockEffect;
    FreezeEffect _freezeEffect;

    PauseGame _pauseGame;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _burnEffect = GetComponent<BurnEffect>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _shockEffect = GetComponent<ShockEffect>();
        _freezeEffect = GetComponent<FreezeEffect>();

        _pauseGame = FindObjectOfType<PauseGame>();
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused && Debug.isDebugBuild)
        {
            ProcessDebugInput();
        }
    }

    void ProcessDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            _burnEffect.ChargeUp();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            _freezeEffect.ChargeUp();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _shockEffect.ChargeUp();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Checks if the enemy is colliding with an projectile
        if (other.GetComponent<Projectile>() != null)
        {
            ProcessFatalCollision(other.GetComponent<Projectile>());
        }
    }

    // Finds the projectile type that is colliding with the enemy
    // If it is a effect projectile (fire, shock, ice), then they charge up
    // If it's not an effect projectile, then it's a commom projectile, which only deals damage to the enemy
    void ProcessFatalCollision(Projectile projectile)
    {
        // Checks if the enemy is dead. This protects against two calls of this method in the same frame
        if (!_enemy.IsAlive) { return; }

        switch (projectile.gameObject.tag)
        {
            case "Burn":
                _burnEffect.ChargeUp();
                break;

            case "Shock":
                _shockEffect.ChargeUp();
                break;

            case "Freeze":
                _freezeEffect.ChargeUp();
                break;

            default:
                _enemyHealth.ProcessHit(projectile.Damage);
                break;
        }
    }
}