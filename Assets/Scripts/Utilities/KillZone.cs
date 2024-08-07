using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] private CheckpointManager checkpointManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform lastCheckpointTransform = checkpointManager.GetLastCheckpointTransform();

            if (!collision.gameObject.GetComponent<PlayerHealth>().ghostMode)
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(100, Vector2.one);
            }
            else
            {
                if (lastCheckpointTransform != null)
                {
                    Vector3 respawnPosition = new Vector3(lastCheckpointTransform.position.x, lastCheckpointTransform.position.y, collision.transform.position.z);
                    collision.GetComponent<PlayerRespawn>().Respawn(respawnPosition);
                }
                else
                {
                    Transform defaultCheckPoint = checkpointManager.GetDefaultCheckpointTransform();
                    Vector3 respawnPosition = new Vector3(defaultCheckPoint.position.x, defaultCheckPoint.position.y, collision.transform.position.z);
                    collision.GetComponent<PlayerRespawn>().Respawn(respawnPosition);
                    // Handle the case where there is no last checkpoint (default respawn or initial position)
                    // For example: respawnPosition = new Vector3(0f, 0f, collision.transform.position.z);
                }
            }
        }
    }
}
