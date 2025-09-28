// Created By: Ryan Lupoli
// This script is designed to manage the bootstrapper ensuring one instance of it is constantly active
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PerformBootstrap
{
    // Name of the Bootstrapper Scene
    const string SceneName = "Bootstrapper";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Execute()
    {
        //  Traverse Currently Loaded Scenes
        for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
        {
            var candidate = SceneManager.GetSceneAt(sceneIndex);

            // Early out if scene is already loaded
            if (candidate.name == SceneName)
            {
                return;
            }
        }

        // Additavely load bootstrapped scene
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
}

public class Bootstrap : MonoBehaviour
{
    // Instance of the Bootstrap Script
    public static Bootstrap Instance { get; private set; } = null;

    void Awake()
    {
        // Check if an instance already exists
        if (Instance != null)
        {
            // If another instance exists, destroy it
            Debug.LogError("Found another instance of BootstrappedData on " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        // Prevent Data from being unloaded
        DontDestroyOnLoad(gameObject);
        Instance = this; // Assign the singleton instance
    }
}

