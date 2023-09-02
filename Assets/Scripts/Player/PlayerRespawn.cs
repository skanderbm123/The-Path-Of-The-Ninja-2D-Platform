using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public static PlayerRespawn Instance { get; private set; }

    [SerializeField] private bool useDefaultRespawn = true;
    [SerializeField] private CheckpointManager checkpointManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (useDefaultRespawn)
            Respawn(checkpointManager.GetDefaultCheckpointTransform().position);
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position; // Use the provided respawn position
        // You might want to reset other player states or behaviors here
    }
}
