// Created By: Ryan Lupoli
// Manages the player's ability to unlock achievements

using UnityEngine;
using System.IO;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    // Runtime copy of achievement data
    public AchievementDatabase database;

    string filePath;

    void Awake()
    {
        // Ensure there is only one instance of the Achievement Manager at any one time
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Ensure this instance of the achievement manager is persistent
        DontDestroyOnLoad(gameObject);

        // Find the file path for the achievements json
        filePath = Path.Combine(Application.persistentDataPath, "achievements.json");
        // Load the achievements
        LoadAchievements();

        // Debug line used to find where the persistent copy of the achivements will be stored.
        //Debug.Log(Application.persistentDataPath);
    }

    void LoadAchievements()
    {
        // If player save exists, load it
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            database = JsonUtility.FromJson<AchievementDatabase>(json);
            Debug.Log("AchievementManager: Loaded achievements from persistent data");
            return;
        }

        // First launch: copy from StreamingAssets
        string streamingPath = Path.Combine(
            Application.streamingAssetsPath,
            "achievements.json"
        );

        if (File.Exists(streamingPath))
        {
            string json = File.ReadAllText(streamingPath);

            //Debug.Log("AchievementManager: STREAMING JSON RAW:\n" + json);

            database = JsonUtility.FromJson<AchievementDatabase>(json);

           // Debug.Log("AchievementManager: Parsed achievements count: {database.achievements.Count}");

            Debug.Log("AchievementManager: Seeded achievements from StreamingAssets");

            // Save seeded data as the player's persistent save
            SaveAchievements();
        }
        else
        {
            // Fallback if Acvhievements JSON is missing (should never be called)
            Debug.LogError("AchievementManager: No achievements.json found in StreamingAssets!");
            database = new AchievementDatabase();
            SaveAchievements();
        }
    }

    void SaveAchievements()
    {
        // Convert the database to a formatted JSON
        string json = JsonUtility.ToJson(database, true);

        // Write JSON text to the save file
        File.WriteAllText(filePath, json);
    }

    // Unlocks an achievement based on the provided id
    // To unlock an achievement in another script use the following line of code: AchievementManager.Instance.UnlockAchievement(achievementId);
    // achievementId is the internal id for the achievement
    public void UnlockAchievement(string id)
    {
        // Find the achivement with the matching id
        Achievement achievement = database.achievements.Find(a => a.id == id);

        // Check to see if the achivement is able to be unlocked

        // If the achievment does not exist
        if (achievement == null)
        {
            Debug.LogWarning("AchievementManager: There is no achivement with the id: " + id +"!");
            // Do nothing
            return;
        }
        // If the achievment is already unlocked
        if (achievement.unlocked)
        {
            Debug.LogWarning("AchievementManager: The achivement " + id + " is already unlocked!");
            // Do nothing
            return;
        }

        // Achivement does exist and is currently locked

        // Mark the achivement as unlocked
        achievement.unlocked = true;

        //Apply metaprogression rewards
        ApplyMetaReward(achievement);

        // Save newly unlocked achivement
        SaveAchievements();

        Debug.Log("AchievementManager: Unlocked: " + id);
    }

    // Resets all of the player's achivements
    [ContextMenu("Reset All Achievements")]
    public void ResetAllAchievements()
    {
        // Check to see if there are any achivements that can be reset
        if (database == null || database.achievements == null)
        {
            Debug.LogWarning("AchievementManager: No achievements to reset.");
            return;
        }

        // Set every achievment's unlock flag to false.
        foreach (Achievement achievement in database.achievements)
        {
            achievement.unlocked = false;
        }

        // Save the achievments
        SaveAchievements();

        Debug.Log("AchievementManager: All achievements have been reset.");
    }

    // Apply the metaprogression rewards for the given achivement
    public void ApplyMetaReward(Achievement achievement)
    {
        return;
    }
}
