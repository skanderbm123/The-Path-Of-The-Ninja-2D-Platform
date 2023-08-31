using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public static PlayerRespawn Instance { get; private set; }

    [SerializeField] private Transform defaultRespawnPosition;
    [SerializeField] private bool useDefaultRespawn = true;

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

    public void Respawn(Vector3 position)
    {
        if (useDefaultRespawn)
        {
            transform.position = defaultRespawnPosition.position; // Use the default respawn
        }
        else
        {
            transform.position = position; // Use the provided respawn position
        }

        // You might want to reset other player states or behaviors here
    }
}
