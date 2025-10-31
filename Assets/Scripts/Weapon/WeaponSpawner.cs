// Created By: Ryan Lupoli
// This weapon allows for the randomized spawning of weapons within a given level.
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour, IInteractable
{
    #region Variables
    // Tracks whether or not a weapon has already been spawned
    private bool weaponSpawned = false;
    private GameObject selectedWeapon;
    private GameObject spawnedWeapon;
    [SerializeField] private bool spawnWeaponOnStart;

    [Header("Probability Settings")]
    [Tooltip("The weighted chance of a spawned weapon being taken from the common weapons pool.")]
    [SerializeField] private int commonWeaponWeight = 60;
    [Tooltip("The weighted chance of a spawned weapon being taken from the uncommon weapons pool.")]
    [SerializeField] private int uncommonWeaponWeight = 30;
    [Tooltip("The weighted chance of a spawned weapon being taken from the rare weapons pool.")]
    [SerializeField] private int rareWeaponWeight = 10;

    [Header("Weapon Pools")]
    [Tooltip("The collection of weapons which make up the common pool for the weapon spawner. Should consist of weapon collectable prefabs.")]
    [SerializeField] private List<GameObject> commonWeaponPool;
    [Tooltip("The collection of weapons which make up the uncommon pool for the weapon spawner. Should consist of weapon collectable prefabs.")]
    [SerializeField] private List<GameObject> uncommonWeaponPool;
    [Tooltip("The collection of weapons which make up the rare pool for the weapon spawner. Should consist of weapon collectable prefabs.")]
    [SerializeField] private List<GameObject> rareWeaponPool;

    [Header("Spawn Level Settings")]
    [Tooltip("Determines how the weapon spawner will assign weapon levels.")]
    public SpawnMode mode;
    // Reference to Player Level manager
    private Player_Level playerLevel;
    // The level of the spawned weapon
    private int spawnedWeaponLevel = 1; // Should never be lower than 1
    [Tooltip("The lowest possible level a weapon can spawn with")]
    [SerializeField] private int minLevel = 1;
    // Highest possible weapon level
    int maxWeaponLevel;

    [Header("Shop Settings")]
    [Tooltip("Determines if the weapon spawner is part of a shop. If true, then weapons cannot be collected unless the player has enough PTO.")]
    [SerializeField] private bool isShop;
    [Tooltip("Determines the cost of the weapon.")]
    [SerializeField] private int weaponPrice;
    [Tooltip("Reference to the canvas for the weapon spawner's price display.")]
    [SerializeField] private GameObject weaponShopCanvas;
    [Tooltip("Reference to the TMPro object which displays the weapons price.")]
    [SerializeField] private TextMeshProUGUI weaponPriceTextbox;
    // Reference to the Economy Manager
    private EconomyManager economyManager;

    [Header("Spawn Location Settings")]
    [Tooltip("Reference to an empty gameobject which functions as a waypoint to define where the generated weapon will be spawned.")]
    [SerializeField] private Transform weaponSpawnWaypoint;

    [Header("DEBUG SETTIGNS")]
    [Tooltip("Overrides the player's level with the specified integer. If value is -1, then the player's actual level will be read")]
    [Range (-1, 10)][SerializeField] private int playerLevelOverride = 5;
    #endregion

    public enum SpawnMode
    {
        // Always spawns weapons at the Players Level
        FixedPL,
        // Spawns a weapon at a random level between the player's current level, and the minLevel of the spawner
        Random
    }

    void Awake()
    {
        playerLevel = FindFirstObjectByType<Player_Level>();
        if (playerLevel == null)
        {
            Debug.LogWarning("Weapon Spawner: Player_Level not found.");
        }

        economyManager = FindFirstObjectByType<EconomyManager>();
        if (playerLevel == null)
        {
            Debug.LogWarning("Weapon Spawner: Economy Manager not found.");
        }

        if (playerLevelOverride == -1)
        {
            maxWeaponLevel = playerLevel.Level;
        }
        else
        {
            maxWeaponLevel = playerLevelOverride;
        }
        // Enable and setup the price display when the spawner is a shop
        if (isShop)
        {
            SetPriceDisplay();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Select a Weapon to Spawn
        selectedWeapon = GetRandomWeapon();

        // If spawner is set to spawn aweapon on start
        if(spawnWeaponOnStart)
        {
            // Spawn a weapon
            SpawnWeapon();
            // If spawner is a shop
            if(isShop)
            {
                // Make weapon un-interactable
                spawnedWeapon.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public void Interact()
    {
        if (isShop)
        {
            // Try to spend the player's money to purchase a weapon
            if (economyManager.SpendPTO(weaponPrice))
            {
                // Disable the WeaponShopCanvas
                weaponShopCanvas.SetActive(false);
                // If the weapon was spawned on start
                if (spawnWeaponOnStart)
                {
                    // Make it interactable
                    spawnedWeapon.layer = LayerMask.NameToLayer("Interactable");
                }
                else
                {
                    // If weapon was not already spawned, spawn it
                    SpawnWeapon();
                }
            }
            return;
        }
        // If a weapon has not spawned, and it is not a shop
        if (!weaponSpawned)
        {
            // Spawn a weapon
            SpawnWeapon();
        }
    }

    private void SpawnWeapon()
    {
        if (weaponSpawnWaypoint != null)
        {
            // Spawn the weapon
            spawnedWeapon = Instantiate(selectedWeapon, weaponSpawnWaypoint.position, weaponSpawnWaypoint.rotation);

            // Find the weapon's script
            IWeapon weaponComponent = spawnedWeapon.GetComponent<IWeapon>();
            // Ensure weapon script was found
            if (weaponComponent != null)
            {
                // Assign the weapon a level
                AssignWeaponLevel(weaponComponent);
                weaponSpawned = true;
                Debug.Log("WeaponSpawner: Spawned a level " + spawnedWeaponLevel + " " + spawnedWeapon.name + ".");
                if(!isShop) gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                Debug.LogWarning("WeaponSpawner: Selected weapon is not part of the IWeapon interface!");
            }
        }
        else
        {
            Debug.LogWarning("WeaponSpawner: Missing WeaponSpawnWaypoint!");
        }
    }

    // Gets a random weapon to be spawned from a random pool
    private GameObject GetRandomWeapon()
    {
        // Generate a random number from between 0 and the combined total of the weapon weights
        int roll = Random.Range(0, commonWeaponWeight + uncommonWeaponWeight + rareWeaponWeight);

        // If the number was less than the common weapon weight...
        if (roll < commonWeaponWeight)
        {
            // Spawned weapon will be taken from the common pool
            return GetRandomFromPool(commonWeaponPool);
        }
        // Else if the number is between the common weapon weight and uncommon weapon weight...
        else if (roll < commonWeaponWeight + uncommonWeaponWeight)
        {
            // Spawned weapon will be taken from the uncommon pool
            return GetRandomFromPool(uncommonWeaponPool);
        }
        // Else if the number is between the uncommon weapon weight and the rare weapon weight...
        else if (roll < commonWeaponWeight + uncommonWeaponWeight + rareWeaponWeight)
        {
            // Spawned weapon will be taken from the rare pool
            return GetRandomFromPool(rareWeaponPool);
        }

        return null;
    }

    // Selects a random weapon from a specified pool
    private GameObject GetRandomFromPool(List<GameObject> pool)
    {
        // If the pool is not populated, return null
        if (pool == null || pool.Count == 0)
        {
            return null;
        }

        // Generate a random number which can be used to select a certain weapon from a pool
        int index = Random.Range(0, pool.Count);
        // Return the selected weapon
        return pool[index];
    }

    // Assigns a weapon a random level from within a range
    private void AssignWeaponLevel(IWeapon weapon)
    {
        // Assign the maximum possible weapon level, which is the same as the player's level
        if (maxWeaponLevel < minLevel)
        {
            maxWeaponLevel = minLevel;
        }

        switch (mode)
        {
            // FixedPL: Spawns a weapon at the player's current level. Intended mostly for testing purposes
            case SpawnMode.FixedPL:
                // Set the weapon's level to the player's level
                weapon.level = maxWeaponLevel;
                spawnedWeaponLevel = maxWeaponLevel;
                break;
            // Random: Spawns a weapon at a random level between the set floor and the player's current level. If resulting level would be less than 1, weapon level is set to 1
            case SpawnMode.Random:
                int range = maxWeaponLevel - minLevel + 1;
                List<int> levels = new List<int>();
                List<float> weights = new List<float>();
                int selectedLevel = 0;

                // Determine the weights for each potential level
                for (int level = minLevel; level <= maxWeaponLevel; level++)
                {
                    // Add the level to the levels list
                    levels.Add(level);
                    // Calculate the weight of the level
                    float weight = Mathf.Pow(level - minLevel + 1, 2);
                    // Add the weight to the weights list in the same position as the corresponding level in the levels list
                    weights.Add(weight);
                }

                // Calculate the total weight
                float totalWeight = 0f;
                foreach (var w in weights)
                {
                    totalWeight += w;
                }

                // Choose a random float between 0 and the total weight
                float randomWeight = Random.Range(0f, totalWeight);

                // Find which level corresponds to the selected weight
                float cumulative = 0f;
                for (int curLevel = 0; curLevel < weights.Count; curLevel++)
                {
                    // Add the weight of curLevel to cumulative
                    cumulative += weights[curLevel];
                    // If the random weight is less than the current cumulative weight...
                    if (randomWeight < cumulative)
                    {
                        // Set the selected level to the current level in the loop
                        selectedLevel = levels[curLevel];
                        // Set the weapon's level to the selected level
                        weapon.level = selectedLevel;
                        spawnedWeaponLevel = selectedLevel;
                        // Break out of the loop
                        return;
                    }
                }
                // As a failsafe set weapon levels to 0 (should never hapepn)
                weapon.level = 0;
                spawnedWeaponLevel = 0;
                break;
        }
    }
    
    // Sets the price display for the weapon spawner. Intended to be used when the spawner is a shop
    private void SetPriceDisplay()
    {
        if (weaponPriceTextbox != null)
        {
            // Enable the canvas
            weaponShopCanvas.SetActive(true);
            // Update the price to textbox to reflect the price
            weaponPriceTextbox.text = string.Format("Price: " + weaponPrice + " PTO");
        }
    }
}
