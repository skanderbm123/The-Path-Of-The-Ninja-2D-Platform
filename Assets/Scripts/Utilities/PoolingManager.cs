using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    // Singleton instance reference.
    public static PoolingManager Instance { get; private set; }

    // Dictionary to store pools of objects by their prefab.
    private Dictionary<GameObject, List<GameObject>> objectPools = new Dictionary<GameObject, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    // Initialize a pool of objects.
    public void InitializePool(GameObject prefab, int initialSize)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new List<GameObject>();
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objectPools[prefab].Add(obj);
            }
        }
    }

    // Spawn an object from the pool.
    public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (objectPools.ContainsKey(prefab))
        {
            List<GameObject> pool = objectPools[prefab];
            foreach (GameObject obj in pool)
            {
                if (!obj.activeInHierarchy)
                {
                    obj.transform.position = position;
                    obj.transform.rotation = rotation;
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        // If no available object in the pool, create a new one.
        GameObject newObj = Instantiate(prefab, position, rotation);
        return newObj;
    }

    // Despawn an object back to the pool.
    public void DespawnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
