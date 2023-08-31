using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private CheckpointManager checkpointManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            checkpointManager.SetLastCheckpoint(transform);
        }
    }
}
