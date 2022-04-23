using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Tooltip("How fast the enemy will move.")]
    [SerializeField] float speed = 1f;
    [Tooltip("How fast the enemy will rotate to look to the next path.")]
    [SerializeField] float rotationSpeed = 10f;
    [Tooltip("how fast the enemy will move based on its speed. 0 means no movement, 1 means 100% movement based on its speed")]
    [SerializeField] [Range(0f, 1f)] float defaultMovementFactor = 1f;

    List<Waypoint> _path = new List<Waypoint>();
    Enemy _enemy;

    float _currentMovementFactor;
    public float MovementFactor
    {
        get => _currentMovementFactor;
        set => _currentMovementFactor = value;
    }

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        _currentMovementFactor = defaultMovementFactor;
        FindPath();
        SetStartPosition();
        StartCoroutine(FollowPath());
    }

    // Find and record the path the enemy will follow throughout his lifetime
    void FindPath()
    {
        // Clear the previous path used in the enemy's previous life
        _path.Clear();

        // Get the transform of an gameobject tagged with "Path", which will be the parent of the "path" gameobjects
        Transform pathTiles = GameObject.FindWithTag("Path").transform;

        // Tterates over all the tiles that are "paths", and add they to the path the enemy will follow
        // The "Transform" has the kinship information of an gameobject and it can be enumerable.
        // That's why the "foreach" works this way with an Transform
        foreach (Transform tile in pathTiles)
        {
            Waypoint waypoint = tile.GetComponent<Waypoint>();

            // Checks if the tile is a proper waypoint
            if (waypoint != null)
            {
                _path.Add(waypoint);
            }
        }
    }

    // Places the enemy in the first waypoint position
    void SetStartPosition()
    {
        if (_path.Count < 1)
        {
            Debug.Log("There is no path to set start position");
            return;
        }

        transform.position = _path[0].transform.position;
    }

    // Called by the FreezeEffect and ShockEffect script when the effect ends
    // When freezing, the enemy's movement factor decreases.
    // When taking shick, the enemy's movement factor is reduced to zero
    // When the effect ends, the multiplier is restored to default
    public void ResetMovementFactor()
    {
        _currentMovementFactor = defaultMovementFactor;
    }

    // Follow the path through time
    IEnumerator FollowPath()
    {
        // Iterates over all the waypoints in the finded path
        // Makes the enemy moves until it reaches the waypoint, and then iterates the next waypoint
        foreach(Waypoint waypoint in _path)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = waypoint.transform.position;
            float travelPercentage = 0f;

            Quaternion startRotation = transform.localRotation;
            Quaternion endRotation;
            Vector3 relativePos = waypoint.transform.position - transform.position;
            float rotationPercentage = 0f;

            endRotation = Quaternion.LookRotation(relativePos);

            if (startRotation != endRotation)
            {
                while (rotationPercentage < 1f)
                {
                    rotationPercentage += Time.deltaTime * rotationSpeed * _currentMovementFactor;
                    transform.localRotation = Quaternion.Slerp(startRotation, endRotation, rotationPercentage);
                    yield return new WaitForEndOfFrame();
                }
            }

            // Makes the enemy move and reach the next waypoint through the time
            // Iterates one time per frame. Keep the game flow running after iterate
            while (travelPercentage < 1f)
            {
                travelPercentage += Time.deltaTime * speed * _currentMovementFactor;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercentage);
                // Return to this method in the next frame. Allow the game to keep running
                yield return new WaitForEndOfFrame();
            }
        }

        FinishPath();
    }

    // Ends the enemy's actions
    void FinishPath()
    {
        _enemy.DamagePlayerCastle();
        _enemy.DeactivateEnemy(false);
    }
}
