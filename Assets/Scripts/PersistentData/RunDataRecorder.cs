// Created By: Ryan Lupoli
using UnityEngine;
using System.IO;

public class RunDataRecorder : MonoBehaviour
{
    public static RunDataRecorder Instance;

    private RunData currentRunData = new RunData();

    private string filePath;

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
    }

    #region Event Management
    private void OnEnable()
    {
        GameEvent.RunStarted += StartNewRun;
        GameEvent.OnEnemyKilled += OnEnemyKilled;
        GameEvent.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvent.RunStarted -= StartNewRun;
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
    #endregion
}
