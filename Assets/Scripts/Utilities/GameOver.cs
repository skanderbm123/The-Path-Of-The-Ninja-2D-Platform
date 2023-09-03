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
        Transform defaultCheckpoint = CheckpointManager.Instance.GetDefaultCheckpointTransform();

        if (lastCheckpoint != null)
        {
            // Move the player to the last checkpoint's position
            PlayerRespawn.Instance.Respawn(lastCheckpoint.position);
        }
        else
        {
            // If there's no last checkpoint, reload the default checkpoint
            PlayerRespawn.Instance.Respawn(defaultCheckpoint.position);
        }
    }
}
