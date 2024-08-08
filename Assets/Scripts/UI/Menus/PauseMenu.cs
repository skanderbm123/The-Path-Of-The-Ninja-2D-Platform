using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button restartButton; // Reference to the restart button in the inspector.

    public void RestartGame()
    {
        // Get the last checkpoint's transform from the CheckpointManager
        Transform lastCheckpoint = CheckpointManager.Instance.GetLastCheckpointTransform();
        Transform defaultCheckpoint = CheckpointManager.Instance.GetDefaultCheckpointTransform();
        if (lastCheckpoint != null)
        {
            //LevelManager.Instance.ResetLevel();
            LevelManager.Instance.RespawnAll();
            PlayerRespawn.Instance.Respawn(lastCheckpoint.position);
        }
        else
        {
            //LevelManager.Instance.ResetLevel();
            LevelManager.Instance.RespawnAll();
            PlayerRespawn.Instance.Respawn(defaultCheckpoint.position);
        }
    }
}
