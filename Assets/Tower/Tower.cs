using System.Collections;
using TMPro;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Tooltip("The effect that is played when the tower dies.")]
    [SerializeField] GameObject deathFX;
    [Tooltip("The effect that is played when the tower is placed at a waypoint.")]
    [SerializeField] GameObject towerPlacedFX;
    [Tooltip("The text that displays the tower's remaining life time.")]
    [SerializeField] TextMeshPro lifeTimeDisplay;
    [Tooltip("The amount of currencies needed to built the tower.")]
    [SerializeField] int creationCost = 4;
    [Tooltip("How long will the tower live.")]
    [SerializeField] int lifeTime = 30;

    Waypoint _waypointSpawned;
    PoolingSystem _deathFxPool;

    int _lifeTimeRemaining;

    public Waypoint WaypointSpawned { set => _waypointSpawned = value; }
    public int CreationCost { get => creationCost; }

    void OnEnable()
    {
        towerPlacedFX.SetActive(true);
        _lifeTimeRemaining = lifeTime;
        StartCoroutine(LifeTimeCountdown());
    }

    void Start()
    {
        PoolingSystemFinder poolingSystemFinder = FindObjectOfType<PoolingSystemFinder>();
        _deathFxPool = poolingSystemFinder.FindPoolingSystem(deathFX);

        DisplayLifetime();
    }

    // Displays the tower's remaining lifetime
    void DisplayLifetime()
    {
        lifeTimeDisplay.text = $"{_lifeTimeRemaining}s";
    }

    // Deactivates the tower after being some time alive
    IEnumerator LifeTimeCountdown()
    {
        DisplayLifetime();
        for (int i = 0; i < lifeTime; i++)
        {
            yield return new WaitForSeconds(1f);
            _lifeTimeRemaining--;
            DisplayLifetime();
        }

        DeactivateTower();
    }

    // Ends the tower's actions and release the waypoint
    public void DeactivateTower()
    {
        // Release the waypoint so another tower can be placed on it.
        _waypointSpawned.IsPlaceable = true;
        _waypointSpawned.TowerPlaced = null;

        PlayVFX(_deathFxPool);
        gameObject.SetActive(false);
    }

    void PlayVFX(PoolingSystem vfxPool)
    {
        if (vfxPool == null) { return; }

        GameObject vfx = vfxPool.GetObject();

        vfx.transform.position = transform.position;
        vfx.transform.rotation = Quaternion.identity;
    }
}
