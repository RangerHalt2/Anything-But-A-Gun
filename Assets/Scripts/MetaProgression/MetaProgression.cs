// Created By: Ryan Lupoli
// Manages the player's ability to unlock achievements

using UnityEngine;

public class MetaProgression : MonoBehaviour
{
    public static MetaProgression Instance;
    
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
}
