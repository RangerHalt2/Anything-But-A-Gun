// Created By: Ryan Lupoli
// Script used to assist in tracking enemey kills across runs for meta progression
using UnityEngine;
using System.IO;
using System.Linq;

public class KillTracker : MonoBehaviour
{
    public static KillTracker Instance { get; private set; }

    private KillDatabase database = new KillDatabase();
    private string filePath;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "kills.json");
        Load();
    }

    // Loads kill data from disk, or creates a new file if none exists.
    void Load()
    {
        if (!File.Exists(filePath))
        {
            Save();
            return;
        }

        string json = File.ReadAllText(filePath);
        database = JsonUtility.FromJson<KillDatabase>(json);
    }

    // Writes kill data to disk.
    void Save()
    {
        string json = JsonUtility.ToJson(database, true);
        File.WriteAllText(filePath, json);
    }

    // Returns total lifetime kills for a given enemy type.
    public int GetKills(string enemyId)
    {
        KillEntry entry = database.kills.FirstOrDefault(k => k.enemyId == enemyId);
        return entry != null ? entry.count : 0;
    }

    // Registers a kill for the given enemy type.
    // Increments the counter, checks achievements, and saves data.
    public void RegisterKill(string enemyId)
    {
        // Find existing entry
        KillEntry entry = database.kills.FirstOrDefault(k => k.enemyId == enemyId);

        // Create new entry if this enemy type hasn't been killed before
        if (entry == null)
        {
            entry = new KillEntry { enemyId = enemyId, count = 0 };
            database.kills.Add(entry);
        }

        // Increment kill count
        entry.count++;

        // Check for achievement unlocks
        CheckAchievements(enemyId, entry.count);
        Save();
    }

    // Checks whether the current kill count meets achievement thresholds.
    void CheckAchievements(string enemyId, int count)
    {
        TryUnlock(enemyId, count, 5);
        TryUnlock(enemyId, count, 25);
        TryUnlock(enemyId, count, 50);
    }

    // Attempts to unlock an achievement for a specific threshold.
    void TryUnlock(string enemyId, int count, int threshold)
    {
        if (count < threshold)
        {
            return;
        }

        // Convert threshold to achievement tier (1, 2, 3)
        string achievementId = $"kill_{enemyId}_{ThresholdToTier(threshold)}";
        AchievementManager.Instance?.UnlockAchievement(achievementId);
    }

    // Converts kill thresholds into achievement tier numbers.
    int ThresholdToTier(int threshold)
    {
        return threshold switch
        {
            5 => 1,
            25 => 2,
            50 => 3,
            _ => 0
        };
    }

    // Resets all tracked enemy kill counts
    [ContextMenu("Reset All Kill Counts")]
    public void ResetAllKillCounts()
    {
        // Safety check
        if (database == null || database.kills == null)
        {
            Debug.LogWarning("KillTracker: No kill data to reset.");
            return;
        }

        // Clear all stored kill entries
        database.kills.Clear();

        // Persist the reset
        Save();

        Debug.Log("KillTracker: All kill counts have been reset.");
    }
}
