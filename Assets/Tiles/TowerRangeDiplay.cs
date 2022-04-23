using System.Collections;
using UnityEngine;

public class TowerRangeDiplay : MonoBehaviour
{
    [Tooltip("The trail that represents the tower's range.")]
    [SerializeField] GameObject trail;

    [Header("Lifetime Behaviour")]
    [Tooltip("How long the trail will live.")]
    [SerializeField] float lifetime = 3f;
    [Tooltip("How fast the trail range will rotate around the tile.")]
    [SerializeField] float angularSpeed = 1000f;

    [Header("After Lifetime Behaviour")]
    [Tooltip("How fast the trail range will decrease after its lifetime expires.")]
    [SerializeField] float deactivationFactor = 30f;
    [Tooltip("How fast the trail range will rotate after the lifetime expires.")]
    [SerializeField] float deactivationSpeed = 1500f;

    PauseGame _pauseGame;
    TowerSelector _towerSelector;

    float _angularMovingFactor;

    void Awake()
    {
        _pauseGame = FindObjectOfType<PauseGame>();
        _towerSelector = FindObjectOfType<TowerSelector>();
    }

    void OnEnable()
    {
        ResetStatus();
        StartCoroutine(LifetimeCountdown());
    }

    void ResetStatus()
    {
        float towerRange = _towerSelector.SelectedTower.GetComponent<TargetLocator>().Range;
        // Sets the trail local position according to the tower's range.
        trail.transform.localPosition = Vector3.right * towerRange;
        _angularMovingFactor = angularSpeed;
    }

    IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(DeactivateTrail());
    }

    // Reduces the trail range over the time.
    IEnumerator DeactivateTrail()
    {
        Vector3 startPosition = trail.transform.localPosition;
        Vector3 endPosition = Vector3.zero;
        float travelPercentage = 0f;

        // Changes the rotation speed when the trail range starts decreasing.
        _angularMovingFactor = deactivationSpeed;

        while (travelPercentage < 1f)
        {
            float travelThisFrame = Time.deltaTime * deactivationFactor;

            travelPercentage += travelThisFrame;
            trail.transform.localPosition = Vector3.Lerp(startPosition, endPosition, travelPercentage);

            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused)
        {
            ProcessRotation();
        }
    }

    // Rotates the trail around the tile.
    // The distance between the trail and the tile represents the "tower range".
    void ProcessRotation()
    {
        float rotationThisFrame = Time.deltaTime * _angularMovingFactor;
        transform.Rotate(Vector3.up * rotationThisFrame);
    }
}
