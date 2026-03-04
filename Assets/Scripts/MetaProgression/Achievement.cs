// Created By: Ryan Lupoli

using System.Data.SqlTypes;
using UnityEngine;

[System.Serializable]
public class Achievement
{
    [Header("Achievement Config")]
    [Tooltip("Internal ID used to identify achievemnets.")]
    public string id;
    [Tooltip("The name of the achievment.")]
    public string name;
    [Tooltip("The flavor text of the achievement.")]
    public string flavorText;
    [Tooltip("The description of the achievement.")]
    public string description;
    [Tooltip("The reward unlocked from beating attaining the achievement.")]
    public string reward;
    [Tooltip("Flag used to determine if a specific achievmenet has been unlocked or not.")]
    public bool unlocked;

    // Meta Progression Rewards
}