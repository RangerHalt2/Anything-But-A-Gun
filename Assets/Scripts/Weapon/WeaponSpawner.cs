// Created By: Ryan Lupoli
// This weapon allows for the randomized spawning of weapons within a given level.
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    #region Variables
    [Header("Probability Settings")]
    // The combined values of common, uncommon, and rareWeaponChance should not total to be more than 100
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

    [Header("Spawning Settings")]
    [Tooltip("Reference to an empty gameobject which functions as a waypoint to define where the generated weapon will be spawned.")]
    [SerializeField] private Transform weaponSpawnWaypoint;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (weaponSpawnWaypoint != null)
        {
            // Generate a random weapon
            GameObject spawnedWeapon = GetRandomWeapon();
            // Check if the weapon was spawned properly
            if (spawnedWeapon != null)
            {
                Instantiate(spawnedWeapon, weaponSpawnWaypoint.position, weaponSpawnWaypoint.rotation);
                Debug.Log("WeaponSpawner: Spawned " + spawnedWeapon.name);
            }
            else
            {
                Debug.LogWarning("WeaponSpawner: Failed to properly spawn weapon");
            }
        }
        else
        {
            Debug.LogWarning("WeaponSpawner: Missing WeaponSpawnWaypoint");
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
}
