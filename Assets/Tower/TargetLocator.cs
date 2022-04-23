using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    [Tooltip("The object that represents the tower's head. that is, the location from which the projectiles are fired.")]
    [SerializeField] Transform weapon;
    [Tooltip("The viewing range of the tower.")]
    [SerializeField] float range = 20f;

    Transform _target;
    Transform _enemyPool;
    PauseGame _pauseGame;
    TowerTrigger _towerTrigger;

    public float Range { get => range; }

    void Awake()
    {
        _pauseGame = FindObjectOfType<PauseGame>();
        _towerTrigger = GetComponent<TowerTrigger>();
        _enemyPool = GameObject.FindWithTag("EnemyPool").transform;
    }

    void Update()
    {
        if (!_pauseGame.IsGamePaused)
        {
            CheckIfTargetIsValid();
            AimAtTarget();
            Attack();
        }
    }

    // Draw the range of the Tower.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Checks if it needs to find another target.
    void CheckIfTargetIsValid()
    {
        bool targetExists = _target != null;
        bool isTargetActivated = false;
        bool isTargetInRange = false;

        // It can only evaluate the other bool variables if the target exists
        if (targetExists)
        {
            float targetDistance = Vector3.Distance(transform.position, _target.position);

            isTargetActivated = _target.gameObject.activeInHierarchy;
            isTargetInRange = targetDistance <= range;
        }

        // Finds another target if at least one of the conditions is not met
        if (!targetExists || !isTargetActivated || !isTargetInRange)
        {
            FindClosestTarget();
        }
    }

    // Search for the closest target that are in the tower's range.
    void FindClosestTarget()
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        // The "Transform" component contains information about the GameObject's kinship.
        // So, by using a Transform in a "foreach" statement, it will return all the children contained in it.
        foreach (Transform enemyPos in _enemyPool)
        {
            if (!enemyPos.gameObject.activeInHierarchy) { continue; }

            float enemyDistance = Vector3.Distance(transform.position, enemyPos.position);
            bool isEnemyInRange = enemyDistance <= range;
            bool isMinDistance = enemyDistance < minDistance;

            if (isEnemyInRange && isMinDistance)
            {
                closestEnemy = enemyPos.transform;
                minDistance = enemyDistance;
            }
        }

        _target = closestEnemy;

        //Enemy[] enemies = FindObjectsOfType<Enemy>();
        //Transform closestEnemy = null;
        //float minEnemyDistance = Mathf.Infinity;

        //foreach (Enemy enemy in enemies)
        //{
        //    float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);

        //    if (enemyDistance <= range && enemyDistance < minEnemyDistance)
        //    {
        //        closestEnemy = enemy.transform;
        //        minEnemyDistance = enemyDistance;
        //    }
        //}

        //_target = closestEnemy;
    }

    // Makes the weapon look at the target.
    void AimAtTarget()
    {
        if (_target == null) { return; }

        weapon.LookAt(_target);
    }

    // Attack the target.
    void Attack()
    {
        if (_target == null) { return; }

        _towerTrigger.Shoot();
    }
}
