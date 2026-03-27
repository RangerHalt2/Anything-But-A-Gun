// Created By Ryan Lupoli
using System.IO;
using Unity.IO;
using UnityEngine;

public class CareerDataRecorder : MonoBehaviour
{
    public static CareerDataRecorder Instance;

    private CareerData currentCareerData = new CareerData();

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

        filePath = Path.Combine(Application.persistentDataPath, "careerData.json");
    }

    #region Event Management
    private void OnEnable()
    {
        GameEvent.RunEnded += RecordRunData;
    }

    private void OnDisable()
    {
        GameEvent.RunEnded -= RecordRunData;
    }
    #region Event Handlers
    private void RecordRunData()
    {
        // Add statistics from the current run to the player's career stats
        currentCareerData.enemiesKilled += RunDataRecorder.Instance.GetEnemiesKilled();
        currentCareerData.levelsCompleted += RunDataRecorder.Instance.GetLevelsCompleted();
        SaveCareerData();
    }
    #endregion
    #endregion

    #region Save / Load
    // Save the current career data to the JSON
    public void SaveCareerData()
    {
        string json = JsonUtility.ToJson(currentCareerData, true);
        File.WriteAllText(filePath, json);
    }

    // Load data from the JSON to the current career
    public void LoadCareerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentCareerData = JsonUtility.FromJson<CareerData>(json);
        }
        else
        {
            currentCareerData = new CareerData();
        }
    }
    #endregion

    [ContextMenu("Reset Career Data")]
    public void ResetCareerData()
    {
        currentCareerData = new CareerData();
        SaveCareerData();
    }

    #region Getters
    public int GetEnemiesKilled()
    {
        return currentCareerData.enemiesKilled;
    }

    public int GetLevelsCompleted()
    {
        return currentCareerData.levelsCompleted;
    }
    #endregion
}
