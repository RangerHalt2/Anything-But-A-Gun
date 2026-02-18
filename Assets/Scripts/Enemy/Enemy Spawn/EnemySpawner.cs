//Author and Contributor: Logan Baysinger
//Purpose: This script handles dynamically for each room the spawning of enemies. This code works in conjunction with SpawnTrigger.cs handling the actual placement and spawning of enemies
//Use: This script needs to be placed on each prefab room that can be spawned with the expected enemies, count, and placements expected of this room.

using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Variables
    [Header("Enemy Spawn Management")]
    [Tooltip("Place the common enemies here, there will be weight to spawn more of these")]
    [SerializeField] private GameObject[] commonEnemies;
    [Tooltip("Place the elite enemies here, there will be almost the minimum accepted amount of these")]
    [SerializeField] private GameObject[] eliteEnemies;
    [Tooltip("This allows you to dictate the bare number of elites required in this room")]
    [SerializeField] private float numReqEliteSpawns;
    [Tooltip("Place the spawn points that you expect the enemies to spawn AT")]
    [SerializeField] private Transform[] spawnPoints;
    [Tooltip("Dictates how many enemies are allowed, if this is greater than the number of spawn points it will simply stop at the spawn points, otherwise if it's less it will only spawn this many. Set this to -1 to fill every spawn point")]
    [SerializeField] private float maxEnemyCount;
    [Tooltip("Common Weight in percentage. 100 is 100% chance, 75 is 75% common enemies")]
    [SerializeField] private float commonEnemyChance = 75;

    [HideInInspector] public List<EnemyClass> enemies;
    [HideInInspector] public List<EnemyClass> aliveEnemies;

    private bool spawnedEnemies = false;
    #endregion

    private void Start()
    {
        if (maxEnemyCount == 0) maxEnemyCount = -1; //Fail Safe
    }

    public void SpawnEnemies()
    {
        if (spawnedEnemies) return;
        int index = 0;
        int eliteCount = 0;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (index == maxEnemyCount) break;

            //Random variables to spawn enemies
            float chance = Random.Range(1f, 100f);
            int commonChoice = Random.Range(0, commonEnemies.Length);
            int eliteChoice = Random.Range(0, eliteEnemies.Length);

            if(eliteEnemies.Length > 0 && ((maxEnemyCount - index) == (numReqEliteSpawns - eliteCount) || (spawnPoints.Length - index) == (numReqEliteSpawns - eliteCount))) //Case to force spawn the elite enemies
            {
                SpawnElite(spawnPoint, eliteChoice);
                eliteCount++;
                index++;
                continue;
            }

            if(chance >= commonEnemyChance && eliteEnemies.Length > 0) //Spawn an elite if the chance is greater than the common enemy chance
            {
                SpawnElite(spawnPoint, eliteChoice);
                eliteCount++;
            }
            else //Spawn a regular enemy
            {
                if(commonEnemies.Length > 0) 
                    SpawnRegular(spawnPoint, commonChoice);
            }
            index++;
        }

        foreach(EnemyClass enemy in enemies)
        {
            aliveEnemies.Add(enemy);
        }

        spawnedEnemies = true;

    }

    //Separated for readability
    private void SpawnElite(Transform spawnPoint, int eliteChoice)
    {
        EnemyClass enemy = Instantiate(eliteEnemies[eliteChoice], spawnPoint.position, spawnPoint.rotation).GetComponent<EnemyClass>();
        enemies.Add(enemy);
    }

    private void SpawnRegular(Transform spawnPoint, int commonChoice)
    {
        EnemyClass enemy = Instantiate(commonEnemies[commonChoice], spawnPoint.position, spawnPoint.rotation).GetComponent<EnemyClass>();
        enemies.Add(enemy);
    }

    //Reset Spawner: MG
    public void ResetSpawner()
    {
        spawnedEnemies = false;
    }

}
