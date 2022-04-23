using UnityEngine;

public class PoolingSystemFinder : MonoBehaviour
{
    // Search, find and return the pooling system containing the respective prefab instances.
    public PoolingSystem FindPoolingSystem(GameObject prefab)
    {
        PoolingSystem[] pools = FindObjectsOfType<PoolingSystem>();

        foreach(PoolingSystem pool in pools)
        {
            if (pool.PoolObject == prefab)
            {
                return pool;
            }
        }

        return null;
    }
}
