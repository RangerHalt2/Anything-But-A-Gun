// Created By: Ryan Lupoli
// This script is intended to store variables meant to persist across scenes. It is intended to be referenced by other scripts
using UnityEngine;

public class BootstrappedData : MonoBehaviour
{
    [Header("Cheats")]
    [Tooltip("Grants the player immortality, preventing any form of death from occuring")]
    [SerializeField] private bool immortalityCheat;

    public bool LevelOneComplete
    {
        get { return immortalityCheat; }
        set { immortalityCheat = value; }
    }
}
