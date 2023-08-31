using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Transform[] checkpoints;
    private Transform lastCheckpointTransform; // Store the last checkpoint's transform

    private void Awake()
    {
        Instance = this;
    }

    public void SetLastCheckpoint(Transform checkpointTransform)
    {
        lastCheckpointTransform = checkpointTransform;
    }

    public Transform GetLastCheckpointTransform()
    {
        return lastCheckpointTransform;
    }

    // Other methods and logic for enabling/disabling checkpoints
}
