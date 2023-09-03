using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    public enum TargetFrameRateOption
    {
        NoLimit = -1,
        FPS30 = 30,
        FPS60 = 60
    }

    public TargetFrameRateOption targetFrameRateOption = TargetFrameRateOption.FPS60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // Disable V-Sync
        Application.targetFrameRate = (int)targetFrameRateOption;
    }
}
