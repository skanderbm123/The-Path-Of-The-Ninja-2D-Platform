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
        Debug.Log((SceneManager.GetActiveScene().name));
        if (lastCheckpoint != null)
        {
            // Move the player to the last checkpoint's position
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            PlayerRespawn.Instance.Respawn(lastCheckpoint.position);
        }
        else
        {
            // If there's no last checkpoint, reload the default checkpoint
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            PlayerRespawn.Instance.Respawn(defaultCheckpoint.position);
        }
    }
}
