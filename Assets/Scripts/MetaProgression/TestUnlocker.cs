// Created By: Ryan Lupoli
// This is a debugging script intended for testing unlocking achivements
using UnityEngine;

public class TestUnlocker : MonoBehaviour
{
    [Header("Achievement")]
    [Tooltip("ID of the achievement to unlock when the player enters this trigger")]
    [SerializeField] private string achievementId;
 
    [Header("Settings")]
    [Tooltip("If true, this trigger only works once")]
    [SerializeField] private bool triggerOnce = true;

    bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // Prevent multiple triggers if set to once
        if (triggerOnce && hasTriggered)
            return;

        // Only react to the player
        if (!other.CompareTag("Player"))
            return;

        // Unlock the achievement
        AchievementManager.Instance.UnlockAchievement(achievementId);
        AchievementManager.Instance.ApplyMetaReward();

        hasTriggered = true;
    }
}
