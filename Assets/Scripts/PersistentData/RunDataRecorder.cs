// Created By: Ryan Lupoli
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RunDataRecorder : MonoBehaviour
{
    public static RunDataRecorder Instance;

    private RunData currentRunData = new RunData();

    private RunData previousRunData = new RunData();
    public bool HasPreviousRunData { get; private set; } = false;

    private string filePath;

    private string previousRunFilePath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filePath = Path.Combine(Application.persistentDataPath, "runData.json");
        previousRunFilePath = Path.Combine(Application.persistentDataPath, "previousRunData.json");

        if (File.Exists(previousRunFilePath))
        {
            string json = File.ReadAllText(previousRunFilePath);
            previousRunData = JsonUtility.FromJson<RunData>(json);

            HasPreviousRunData = previousRunData.weaponsCollected != null && previousRunData.weaponsCollected.Count > 0;
        }
    }

    #region Event Management
    private void OnEnable()
    {
        GameEvent.RunEnded += StartNewRun;
        GameEvent.OnEnemyKilled += OnEnemyKilled;
        GameEvent.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvent.RunEnded -= StartNewRun;
        GameEvent.OnEnemyKilled -= OnEnemyKilled;
        GameEvent.OnLevelCompleted -= OnLevelCompleted;
    }
    #region Event Handlers
    private void OnEnemyKilled()
    {
        currentRunData.enemiesKilled++;
        SaveRunData();
    }

    private void OnLevelCompleted(string id)
    {
        currentRunData.levelsCompleted++;
        SaveRunData();
    }

    // Weapon Records
    public void RecordWeapon(string weaponID)
    {
        if (!currentRunData.weaponsCollected.Contains(weaponID))
        {
            currentRunData.weaponsCollected.Add(weaponID);
            SaveRunData();
        }
    }

    public List<string> GetWeapons()
    {
        return currentRunData.weaponsCollected;
    }

    #endregion
    #endregion

    #region Save / Load
    // Save the current run data to the JSON
    public void SaveRunData()
    {
        string json = JsonUtility.ToJson(currentRunData, true);
        File.WriteAllText(filePath, json);
    }

    // Load data from the JSON to the current run
    public void LoadRunData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentRunData = JsonUtility.FromJson<RunData>(json);
        }
        else
        {
            currentRunData = new RunData();
        }
    }
    #endregion

    // Resets the RunData JSON
    // Intended to be called at the start of every run
    public void StartNewRun()
    {
        Debug.Log("Run Data Recorder: StartNewRun CALLED");
        // Save CURRENT run as previous run snapshot
        string json = JsonUtility.ToJson(currentRunData, true);
        File.WriteAllText(previousRunFilePath, json);

        // Load into memory as previous run too
        previousRunData = JsonUtility.FromJson<RunData>(json);
        HasPreviousRunData = previousRunData.weaponsCollected != null && previousRunData.weaponsCollected.Count > 0;

        currentRunData = new RunData();
        SaveRunData();
    }

    #region Getters
    public int GetEnemiesKilled()
    {
        return currentRunData.enemiesKilled;
    }

    public int GetLevelsCompleted()
    {
        return currentRunData.levelsCompleted;
    }

    public List<string> GetPreviousRunWeapons()
    {
        return previousRunData.weaponsCollected;
    }
    #endregion
}
