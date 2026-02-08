using UnityEngine;

public class MetaResetButton : MonoBehaviour
{
    public void ResetAllMetaProgression()
    {
        // Reset achievements
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.ResetAllAchievements();
        }
        else
        {
            Debug.LogWarning("MetaResetButton: AchievementManager not found.");
        }

        // Reset kill counts
        if (KillTracker.Instance != null)
        {
            KillTracker.Instance.ResetAllKillCounts();
        }
        else
        {
            Debug.LogWarning("MetaResetButton: KillTracker not found.");
        }

        Debug.Log("MetaResetButton: All meta-progression reset.");
    }
}
