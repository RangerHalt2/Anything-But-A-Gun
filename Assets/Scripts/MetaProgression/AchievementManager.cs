// Created By: Ryan Lupoli
// Manages the player's ability to unlock achievements

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    // Runtime copy of achievement data
    public AchievementDatabase database;

    private string lastAchievementID;

    string filePath;

    [SerializeField]
    private List<string> masterWeaponList = new List<string>()
    {
        "Breadcrumbs",
        "Fire Extinguisher",
        "Baseball Bat",
        "Cat and Toast",
        "Windbreaker",
        "Volleyball",
        "Cactus",
        "Megaphone",
        "Laser Pointer",
        "Divorce Papers",
        "Air Frier",
        "Nose Dive",
        "Paper Shuriken",
        "Brush",
        "Camera",
        "Duck Duck Goose!",
        "Knight",
        "Lava Lamp"
    };

    [Header("DEBUG/CHEATS")]
    [Tooltip("For testing purposes only. Allows for the achievment menu to lock or unlock a given achievement by holding Ctrl + Shift when clicking on one of the achievments")]
    [SerializeField] bool toggleAchivementState = false;

    void Awake()
    {
        // Ensure there is only one instance of the Achievement Manager at any one time
        if (Instance != null)
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
        Debug.Log(Application.persistentDataPath);
    }

    #region Achievement Management
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

    // Getter method for achievements
    public Achievement GetAchievement(string id)
    {
        if (database == null || database.achievements == null)
        {
            Debug.LogWarning("Achivement Manager: Get Achievement called with invalid id!");
            return null;
        }

        return database.achievements.Find(a => a.id == id);
    }

    public bool CheckAchivementStatus(string id)
    {
        if (database == null || database.achievements == null)
        {
            Debug.LogWarning("Achivement Manager: Check Achievement Status called with invalid id!");
            return false;
        }
        // Find the achivement in the database
        Achievement a = database.achievements.Find(a => a.id == id);

        if (a == null)
        {
            Debug.LogWarning("AchievementManager: Achievement " + id + " not found.");
            return false;
        }

        return a.unlocked;
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
            Debug.LogWarning("AchievementManager: There is no achivement with the id: " + id + "!");
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
        // Record the ID of the last achievment unlocked so other scripts can easily access it
        lastAchievementID = id;

        //Apply metaprogression rewards
        GameEvent.OnAchivementEarned?.Invoke();

        // Save newly unlocked achivement
        SaveAchievements();

        Debug.Log("AchievementManager: Unlocked: " + id);
    }
    #endregion

    #region Event Management
    private void OnEnable()
    {
        GameEvent.OnLevelCompleted += HandleLevelCompleted;
        GameEvent.OnEnemyKilled += HandleEnemyKill;
        GameEvent.OnWeaponModified += HandlePromotion;
        GameEvent.StyleMaxxed += HandleStyleMaxxed;
        GameEvent.spendPTO += HandleConsumerism;
        GameEvent.OnWeaponPickup += CheckAllWeaponsCollected;
    }

    private void OnDisable()
    {
        GameEvent.OnLevelCompleted += HandleLevelCompleted;
    }

    #region Event Handlers
    // Attempts to unlock achievements when levels are completed
    void HandleLevelCompleted(string levelID)
    {
        if (levelID == "office")
            UnlockAchievement("beat_first_level");

        if (levelID == "lab")
            UnlockAchievement("beat_second_level");

        if (CareerDataRecorder.Instance.GetLevelsCompleted() + RunDataRecorder.Instance.GetLevelsCompleted() >= 5)
        {
            UnlockAchievement("wave_master");
        }
    }

    void HandleEnemyKill()
    {
        UnlockAchievement("beat_first_enemy");
    }

    void HandlePromotion()
    {
        UnlockAchievement("modify_weapon");
    }

    void HandleStyleMaxxed()
    {
        UnlockAchievement("max_style");
    }

    void HandleConsumerism()
    {
        UnlockAchievement("consumer");
    }

    void CheckAllWeaponsCollected(GameObject weapon)
    {
        HashSet<string> combined = new HashSet<string>();

        // Add run weapons
        foreach (var w in RunDataRecorder.Instance.GetWeapons())
            combined.Add(w);

        // Add career weapons
        foreach (var w in CareerDataRecorder.Instance.GetWeapons())
            combined.Add(w);

        // Check against master list
        foreach (var required in masterWeaponList)
        {
            if (!combined.Contains(required))
            {
                return; // Missing at least one
            }
        }

        // All collected
        UnlockAchievement("weapon_master");
    }

    #endregion
    #endregion

    public string GetLastAchievement()
    {
        return lastAchievementID;
    }

    #region Debug
    // Getter method for toggleAchievmentState. Intended for debug purposes only
    public bool canToggle()
    {
        return toggleAchivementState;
    }

    // Toggles the state of an achievement between its locked and unlocked state. Intended for debug purposes only
    public void ToggleAchievement(string id)
    {
        // Find an achievment in the database whose id matches the provided id
        Achievement achievement = database.achievements.Find(a => a.id == id);

        // If no achievmeent was found, do nothing
        if (achievement == null)
        {
            Debug.LogWarning("AchievementManager: No achievement with id: " + id);
            return;
        }

        // Toggle the state of the achievment
        achievement.unlocked = !achievement.unlocked;
        GameEvent.OnAchivementEarned?.Invoke();

        SaveAchievements();
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
    #endregion

    // Apply the metaprogression rewards for the given achivement
    public void ApplyMetaReward()
    {        
        PlayerMetaStats playerMeta = FindFirstObjectByType<PlayerMetaStats>();

        if (playerMeta != null)
        {
            playerMeta.OnMetaAchievementUnlocked();
        }
    }
}
