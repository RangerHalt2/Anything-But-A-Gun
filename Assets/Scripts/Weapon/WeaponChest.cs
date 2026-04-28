// Created By: Ryan Lupoli
// A Specialized version of the Weapon Spawner Script designed to allow for multiple weapons to be spawned at once, with the player only being allowed to select 1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class WeaponChest : MonoBehaviour, IInteractable
{
    #region Variables
    private bool weaponsSpawned = false;

    [Header("Spawn Count")]
    [Tooltip("How many weapons will spawn when the chest is opened normally.")]
    [SerializeField] private int numberOfWeaponsToSpawn = 2;
    [Tooltip("How many weapons will spawn when the chest is opened with the Crunch Culture Achievement unlocked.")]
    [SerializeField] private int numberOfExtraWeaponsToSpawn = 3;

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

    [Header("Spawn Settings")]
    [Tooltip("Reference to a list of empty gameobjects which function as a waypoints to define where the generated weapons will be spawned.")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Level Settings")]
    public WeaponSpawner.SpawnMode mode;
    private Player_Level playerLevel;

    [SerializeField] private int minLevel = 1;
    private int maxWeaponLevel;

    [Header("DEBUG")]
    [Tooltip("Determnies whether or not this is the starter chest.")]
    [SerializeField] private bool starterChest = false;
    [Tooltip("Determnies whether or not this is the retaining chest.")]
    [SerializeField] private bool retainingChest = false;
    [Tooltip("Overrides the player's level with the specified integer. If value is -1, then the player's actual level will be read")]
    [Range(-1, 10)][SerializeField] private int playerLevelOverride = -1;
    [SerializeField] private bool forceSpawn = true;

    private List<GameObject> spawnedWeapons = new List<GameObject>();

    [SerializeField] private string[] denials = new string[0];
    public string[] denyText { get { return denials; } set { denials = value; } }

    public bool canInteract { get; set; } = true;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerLevel = FindFirstObjectByType<Player_Level>();

        if (playerLevelOverride == -1)
        {
            maxWeaponLevel = playerLevel.Level;
        }
        else
        {
            maxWeaponLevel = playerLevelOverride;
        }

        if (AchievementManager.Instance != null)
        {
            if (AchievementManager.Instance.CheckAchivementStatus("wave_master"))
            {
                numberOfWeaponsToSpawn = numberOfExtraWeaponsToSpawn;
            }
        }
    }

    void Start()
    {
        StartCoroutine(SpawnWeaponsDelayed());
    }

    private IEnumerator SpawnWeaponsDelayed()
    {
        yield return null;

        SpawnWeapons();

        if (AchievementManager.Instance != null)
        {
            // If the player has not obtained the achievement Nine-To-Five (ID: beat_first_level) and the promotion is not set to be force spawned
            if (!AchievementManager.Instance.CheckAchivementStatus("beat_first_level") && !forceSpawn)
            {
                DestroySpawnedWeapons();

                Destroy(gameObject);
                yield break;
            }
        }

        var recorder = RunDataRecorder.Instance;
    }

    private void OnEnable()
    {
        GameEvent.OnWeaponPickup += HandleWeaponPicked;
    }

    private void OnDisable()
    {
        GameEvent.OnWeaponPickup -= HandleWeaponPicked;
    }

    public void Interact()
    {
        if (!weaponsSpawned)
        {
            SpawnWeapons();
        }
    }

    private void SpawnWeapons()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("WeaponChest: No spawn points assigned!");
            return;
        }

        // Ensure that there will be enough spwan points for the desired amount of weapons by using the lower of the two values
        int spawnCount = Mathf.Min(numberOfWeaponsToSpawn, spawnPoints.Length);

        List<GameObject> usedPrefabs = new List<GameObject>();
        if(retainingChest)
        {
            var recorder = RunDataRecorder.Instance;
            if (recorder == null || !recorder.HasPreviousRunData)
            {
                Debug.Log("WeaponChest: No previous run data found. Destroying retaining chest.");

                Destroy(gameObject);
                return;
            }
            SpawnRetainedWeapons(spawnCount, usedPrefabs);
        }
        else
        {
            for (int i = 0; i < spawnCount; i++)
            {
                // Select a random Weapon
                GameObject selectedWeapon = GetRandomWeaponNoDupes(usedPrefabs);

                if (selectedWeapon == null)
                    continue;

                // Instantiate the weapon at one of the spawn points
                GameObject weapon = Instantiate(selectedWeapon, spawnPoints[i].position, spawnPoints[i].rotation);

                // Add the Weapon to the list of spwanedWeapons
                spawnedWeapons.Add(weapon);
                usedPrefabs.Add(selectedWeapon);
            }
        }
        weaponsSpawned = true;
    }

    private void HandleWeaponPicked(GameObject pickedWeapon)
    {
        // Only react if the current chest spawned the weapon
        if (!spawnedWeapons.Contains(pickedWeapon))
        {
            return;
        }

        // Destroy all spawned weapons
        foreach (var weapon in spawnedWeapons)
        {
            if (weapon != null && weapon != pickedWeapon)
            {
                Destroy(weapon);
            }
        }

        // Clear the spwaend weapons list
        spawnedWeapons.Clear();
    }

    #region Weapon Selection

    private GameObject GetRandomWeaponNoDupes(List<GameObject> used)
    {
        // Build filtered pools (remove used weapons)
        List<GameObject> common = commonWeaponPool.FindAll(w => !used.Contains(w));
        List<GameObject> uncommon = uncommonWeaponPool.FindAll(w => !used.Contains(w));
        List<GameObject> rare = rareWeaponPool.FindAll(w => !used.Contains(w));

        // Build dynamic weights (only include non-empty pools)
        int totalWeight = 0;

        if (common.Count > 0) totalWeight += commonWeaponWeight;
        if (uncommon.Count > 0) totalWeight += uncommonWeaponWeight;
        if (rare.Count > 0) totalWeight += rareWeaponWeight;

        if (totalWeight == 0)
        {
            return null; // no unique weapons left
        }
            
        int roll = Random.Range(0, totalWeight);

        // Select rarity based on remaining valid pools
        if (common.Count > 0)
        {
            if (roll < commonWeaponWeight)
            {
                return common[Random.Range(0, common.Count)];
            }

            roll -= commonWeaponWeight;
        }

        if (uncommon.Count > 0)
        {
            if (roll < uncommonWeaponWeight)
            {
                return uncommon[Random.Range(0, uncommon.Count)];
            }

            roll -= uncommonWeaponWeight;
        }

        // fallback to rare
        if (rare.Count > 0)
        {
            return rare[Random.Range(0, rare.Count)];
        }

        return null;
    }

    private GameObject GetRandomWeapon()
    {
        int roll = Random.Range(0, commonWeaponWeight + uncommonWeaponWeight + rareWeaponWeight);

        if (roll < commonWeaponWeight)
        {
            return GetRandomFromPool(commonWeaponPool);
        }
        else if (roll < commonWeaponWeight + uncommonWeaponWeight)
        {
            return GetRandomFromPool(uncommonWeaponPool);
        }
        else
        {
            return GetRandomFromPool(rareWeaponPool);
        }
    }

    private GameObject GetRandomFromPool(List<GameObject> pool)
    {
        if (pool == null || pool.Count == 0)
        {
            return null;
        }

        return pool[Random.Range(0, pool.Count)];
    }

    #endregion

    private void DestroySpawnedWeapons()
    {
        foreach (var weapon in spawnedWeapons)
        {
            if (weapon != null)
            {
                Destroy(weapon);
            }
        }

        spawnedWeapons.Clear();
    }

    #region Previous Run Chest Logic
    // Used to identify weapons in the chest's pools by name
    private List<GameObject> GetWeaponsByName(string weaponName)
    {
        List<GameObject> matches = new List<GameObject>();

        List<GameObject> allPools = new List<GameObject>();
        allPools.AddRange(commonWeaponPool);
        allPools.AddRange(uncommonWeaponPool);
        allPools.AddRange(rareWeaponPool);

        foreach (var weapon in allPools)
        {
            WeaponClass wc = weapon.GetComponent<WeaponClass>();
            if (wc != null && wc.GetWeaponName() == weaponName)
            {
                matches.Add(weapon);
            }
        }

        return matches;
    }

    // Gets a weapon that is not found in the list of names
    private GameObject GetWeaponNotFromNames(List<string> excludedNames, List<GameObject> used)
    {
        List<GameObject> allWeapons = new List<GameObject>();
        allWeapons.AddRange(commonWeaponPool);
        allWeapons.AddRange(uncommonWeaponPool);
        allWeapons.AddRange(rareWeaponPool);

        List<GameObject> valid = new List<GameObject>();

        foreach (var weapon in allWeapons)
        {
            WeaponClass wc = weapon.GetComponent<WeaponClass>();

            if (wc == null) continue;

            if (!excludedNames.Contains(wc.GetWeaponName()) && !used.Contains(weapon))
            {
                valid.Add(weapon);
            }
        }

        if (valid.Count == 0)
            return null;

        return valid[Random.Range(0, valid.Count)];
    }

    // Converts a list of weapon names to a list of weapon prefabs found within the chest's pools
    private List<GameObject> ConvertNamesToPrefabs(List<string> weaponNames)
    {
        List<GameObject> results = new List<GameObject>();

        foreach (var name in weaponNames)
        {
            List<GameObject> matches = GetWeaponsByName(name);

            if (matches.Count > 0)
            {
                GameObject randomVariant = matches[Random.Range(0, matches.Count)];
                results.Add(randomVariant);
            }
        }

        return results;
    }

    // Spawns weapons, attempting to take two from the player's prior run and one which they did not obtain
    private void SpawnRetainedWeapons (int spawnCount, List<GameObject> usedPrefabs)
    {
        var recorder = RunDataRecorder.Instance;

        List<string> prevNames = (recorder != null && recorder.HasPreviousRunData)
            ? recorder.GetPreviousRunWeapons()
            : new List<string>();

        // The list of weapon prefabs created from the list of weapon names
        List<GameObject> prevPrefabs = ConvertNamesToPrefabs(prevNames);

        //
        for (int i = 0; i < prevPrefabs.Count; i++)
        {
            int rand = Random.Range(0, prevPrefabs.Count);
            (prevPrefabs[i], prevPrefabs[rand]) = (prevPrefabs[rand], prevPrefabs[i]);
        }

        int index = 0;
        
        int retainedCount = Mathf.Min(2, prevPrefabs.Count);
        // Spawn up to two retained weapons
        for (int i = 0; i < retainedCount && index < spawnCount; i++, index++)
        {
            SpawnWeapon(prevPrefabs[i], index, usedPrefabs);
        }

        // Fill remaining slots with random weapons
        while(index < spawnCount)
        {
            GameObject fallbackWeapon;

            if (prevPrefabs.Count == 0)
            {
                // no prior data -> pure random
                fallbackWeapon = GetRandomWeaponNoDupes(usedPrefabs);
            }
            else
            {
                fallbackWeapon = GetWeaponNotFromNames(prevNames, usedPrefabs);

                if (fallbackWeapon == null)
                {
                    fallbackWeapon = GetRandomWeaponNoDupes(usedPrefabs);
                }
            }

            SpawnWeapon(fallbackWeapon, index, usedPrefabs);
            index++;
        }
    }

    private void SpawnWeapon(GameObject prefab, int index, List<GameObject> usedPrefabs)
    {
        if (prefab == null)
            return;

        if (index >= spawnPoints.Length)
        {
            Debug.LogWarning("WeaponChest: Spawn index out of bounds!");
            return;
        }

        // Instantiate at the correct spawn point
        GameObject weaponInstance = Instantiate(
            prefab,
            spawnPoints[index].position,
            spawnPoints[index].rotation
        );

        // Track it
        spawnedWeapons.Add(weaponInstance);
        usedPrefabs.Add(prefab);
    }
    #endregion
}