using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private ObjectStateTracker[] objectsToRespawn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Automatically find all objects in the scene with ObjectStateTracker
        objectsToRespawn = FindObjectsOfType<ObjectStateTracker>();
    }

    public void RespawnAll()
    {
        foreach (ObjectStateTracker tracker in objectsToRespawn)
        {
            tracker.ResetToInitialState();
        }
    }
}
