using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    public enum TargetFrameRateOption
    {
        NoLimit = -1,
        FPS30 = 25,
        FPS60 = 55
    }

    public TargetFrameRateOption targetFrameRateOption = TargetFrameRateOption.FPS60;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // Disable V-Sync
        Application.targetFrameRate = (int)targetFrameRateOption;
    }
}
