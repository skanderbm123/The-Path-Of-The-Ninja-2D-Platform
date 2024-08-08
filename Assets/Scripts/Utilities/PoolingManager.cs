using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    // Singleton instance reference.
    public static PoolingManager Instance { get; private set; }

    public GameObject prefab;
    public int poolSize = 20;

    private Queue<GameObject> poolQueue;

    // This dictionary will store initial positions and rotations
    private Dictionary<GameObject, (Vector3, Quaternion)> initialTransforms;

    public List<GameObject> activeObjects = new List<GameObject>();

    void Start()
    {
        poolQueue = new Queue<GameObject>();
        initialTransforms = new Dictionary<GameObject, (Vector3, Quaternion)>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);

            // Store the initial transform data
            initialTransforms[obj] = (obj.transform.position, obj.transform.rotation);
        }
    }

    public GameObject GetFromPool(Vector3? position = null, Quaternion? rotation = null)
    {
        GameObject obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
            initialTransforms[obj] = (obj.transform.position, obj.transform.rotation);
        }

        obj.SetActive(true);
        obj.transform.position = position ?? initialTransforms[obj].Item1;
        obj.transform.rotation = rotation ?? initialTransforms[obj].Item2;
        activeObjects.Add(obj);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
        activeObjects.Remove(obj);
    }

    public void ResetAllObjects()
    {
        foreach (GameObject obj in activeObjects)
        {
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
        activeObjects.Clear();
    }
}
