using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Transform[] checkpoints;
    private Transform lastCheckpointTransform; // Store the last checkpoint's transform
    private Transform defaultCheckpointTransform;  // Store the first (default) checkpoint's transform
    private void Awake()
    {
        Instance = this;

        // Check if there are checkpoints in the array and assign the first one as the default
        if (checkpoints != null && checkpoints.Length > 0)
        {
            defaultCheckpointTransform = checkpoints[0];
        }
        else
        {
            Debug.LogWarning("No checkpoints found in the array. Set some in the Inspector.");
        }
    }

    public void SetLastCheckpoint(Transform checkpointTransform)
    {
        lastCheckpointTransform = checkpointTransform;
    }

    public Transform GetLastCheckpointTransform()
    {
        return lastCheckpointTransform;
    }

    public Transform GetDefaultCheckpointTransform()
    {
        return defaultCheckpointTransform;
    }
    // Other methods and logic for enabling/disabling checkpoints
}
