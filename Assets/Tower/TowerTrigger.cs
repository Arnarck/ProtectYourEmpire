using System.Collections;
using UnityEngine;

public class TowerTrigger : MonoBehaviour
{
    [Tooltip("The projectile's cooldown time.")]
    [SerializeField] float spawnTimer = 1f;

    [Header("Game Objects")]
    [Tooltip("The projectile that will be fired by this tower.")]
    [SerializeField] GameObject projectilePrefab;
    [Tooltip("The effect that is played when a projectile is fired.")]
    [SerializeField] GameObject muzzleFlashFX;

    [Header("Coordinates")]
    [Tooltip("The coordinates where the projectile will be spawned.")]
    [SerializeField] Transform projectileCoordinates;
    [Tooltip("The coordinates where the muzzle flash will be spawned.")]
    [SerializeField] Transform muzzleFlashCoordinates;

    PoolingSystem _projectilePool, _muzzleFlashFxPool;

    bool _canShoot;

    void OnEnable()
    {
        // Changes the value of "canShoot" to "true" every time the enemy RESPAWNS.
        // The first time the enemy spawns,
        // "_canShoot" can only be true after "_projectilePool" and "_muzzleFlashPool" receive their references.
        // That's why the "if" statement is needed here.
        if (_projectilePool != null && _muzzleFlashFxPool != null)
        {
            _canShoot = true;
        }
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _projectilePool = poolingSystemFinder.FindPoolingSystem(projectilePrefab);
        _muzzleFlashFxPool = poolingSystemFinder.FindPoolingSystem(muzzleFlashFX);

        _canShoot = true;
    }

    // Fires a projectile.
    public void Shoot()
    {
        if (!_canShoot) { return; }

        _canShoot = false;

        PlayVFX(_projectilePool, projectileCoordinates);
        PlayVFX(_muzzleFlashFxPool, muzzleFlashCoordinates);
        StartCoroutine(CoolDown());
    }

    void PlayVFX(PoolingSystem vfxPool, Transform coordinates)
    {
        if (vfxPool == null) { return; }

        GameObject vfx = vfxPool.GetObject();

        vfx.transform.position = coordinates.position;
        vfx.transform.rotation = coordinates.rotation;
    }

    // The time until the tower can fire again.
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(spawnTimer);
        _canShoot = true;
    }
}
