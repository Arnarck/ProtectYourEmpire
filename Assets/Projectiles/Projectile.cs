using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("The effect that is played when the enemy collides with something.")]
    [SerializeField] GameObject collisionFX;

    [Header("Projectile Settings")]
    [Tooltip("How fast the projectile will move.")]
    [SerializeField] float speed = 50f;
    [Tooltip("The damage the projectile inflicts on the enemy. Only works if the projectile is a Bolt.")]
    [SerializeField] int damage = 1;

    PauseGame _pauseGame;
    PoolingSystem _collisionFxPool;

    public int Damage { get => damage; }

    void Awake()
    {
        _pauseGame = FindObjectOfType<PauseGame>();
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _collisionFxPool = poolingSystemFinder.FindPoolingSystem(collisionFX);
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused)
        {
            ProcessMovement();
        }
    }

    // Makes the projectile move
    void ProcessMovement()
    {
        float speedThisFrame = Time.deltaTime * speed;
        transform.Translate(Vector3.forward * speedThisFrame, Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        // Deactivates the projectile if it touches everything, except another projectile
        if (other.GetComponent<Projectile>() == null)
        {
            DeactivateProjectile();
        }
    }

    // Deactivates the projectile
    void DeactivateProjectile()
    {
        PlayVFX(_collisionFxPool);
        gameObject.SetActive(false);
    }

    void PlayVFX(PoolingSystem vfxPool)
    {
        if (vfxPool == null) { return; }

        GameObject vfx = vfxPool.GetObject();

        vfx.transform.position = transform.position;
        vfx.transform.rotation = transform.rotation;
    }
}
