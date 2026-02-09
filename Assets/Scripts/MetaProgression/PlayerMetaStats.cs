// Created By: Ryan Lupoli
// Handles the adjustment of player stats based on maximum health
using UnityEngine;
using System.Linq;

public class PlayerMetaStats : MonoBehaviour
{
    [Header("Base Health Settings")]
    public float baseMaxHealth = 100f;
    public float bonusPerAchievement = 5f;

    private Health health;

    // Achievement IDs that grant max health
    private readonly string[] healthAchievements =
    {
        "kill_brick_1",
        "kill_brick_2",
        "kill_brick_3",

        "kill_beanpole_1",
        "kill_beanpole_2",
        "kill_beanpole_3",

        "kill_boomba_1",
        "kill_boomba_2",
        "kill_boomba_3"
    };

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void Start()
    {
        ApplyMetaHealth(true);
    }

    /// Recalculate max health from unlocked achievements
    public void ApplyMetaHealth(bool healToFull)
    {
        // Check to ensure there is both an achievmeent manager instance and health component
        if (AchievementManager.Instance == null || health == null)
        {
            return;
        }
            
        int unlockedCount = AchievementManager.Instance.database.achievements.Count(a => a.unlocked && healthAchievements.Contains(a.id));

        // Calculate the current amount of bonus health
        float bonusHealth = unlockedCount * bonusPerAchievement;
        // Calculate the new max health of the player
        float newMaxHealth = baseMaxHealth + bonusHealth;

        // Set the player's new maximum health
        health.setMaxHealth(newMaxHealth, healToFull);

        Debug.Log("PlayerMetaStats: Max health increased to " + newMaxHealth);
    }

    /// Call this when a new achievement unlocks mid-run
    public void OnMetaAchievementUnlocked()
    {
        // Keep player health relative (false)
        ApplyMetaHealth(false);
    }
}