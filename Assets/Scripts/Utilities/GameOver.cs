using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Button restartButton; // Reference to the restart button in the inspector.

    public void RestartGame()
    {
        // Get the last checkpoint's transform from the CheckpointManager
        Transform lastCheckpoint = CheckpointManager.Instance.GetLastCheckpointTransform();
        PlayerHealth playerHealth = PlayerHealth.Instance;
        if (lastCheckpoint != null)
        {
            // Move the player to the last checkpoint's position
            PlayerRespawn.Instance.Respawn(lastCheckpoint.position);
            playerHealth.Heal(playerHealth.maxHealth);
        }
        else
        {
            // If there's no last checkpoint, reload the default checkpoint
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
