// Created By: Ryan Lupoli
// This script is intended to be used in order to quickly reset all persistent data Jsons
using UnityEngine;

public class ResetData : MonoBehaviour
{
    public static ResetData Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Resets ALL persistent player progression data
    [ContextMenu("Reset ALL Persistent Data")]
    public void ResetAllData()
    {
        ResetAchievements();
        ResetKillData();
        ResetCareerData();

        Debug.Log("ResetData: All persistent player data has been reset.");
    }

    void ResetAchievements()
    {
        if (AchievementManager.Instance != null)
        {
            AchievementManager.Instance.ResetAllAchievements();
            Debug.Log("ResetData: Achievements reset.");
        }
        else
        {
            Debug.LogWarning("ResetData: AchievementManager not found.");
        }
    }

    void ResetKillData()
    {
        if (KillTracker.Instance != null)
        {
            KillTracker.Instance.ResetAllKillCounts();
            Debug.Log("ResetData: Kill tracker reset.");
        }
        else
        {
            Debug.LogWarning("ResetData: KillTracker not found.");
        }
    }

    void ResetCareerData()
    {
        if (CareerDataRecorder.Instance != null)
        {
            CareerDataRecorder.Instance.ResetCareerData();
            Debug.Log("ResetData: Career data reset.");
        }
        else
        {
            Debug.LogWarning("ResetData: CareerDataRecorder not found.");
        }
    }
}
