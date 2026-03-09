using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<EnemySpawner> spawners;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float checkRadius = 50f;
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private float checkInterval = 5f;
    [SerializeField] private float waveInterval = 10f;

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionInterval = 1f;

    [Header("Destroy Door")]
    [SerializeField] private GameObject objectToDestroy;
    [SerializeField] private float destroyDelay = 1.5f;

    private int currentWave = 0;
    private bool active = false;

    void Start()
    {
        InvokeRepeating(nameof(CheckForPlayer), 0f, detectionInterval);
    }

    void CheckForPlayer()
    {
        if (active) return;

        bool playerDetected = Physics.OverlapSphere(transform.position, checkRadius, playerLayer).Length > 0;

        if (playerDetected)
        {
            ActivateWaves();
            CancelInvoke(nameof(CheckForPlayer));
        }
    }

    public void ActivateWaves()
    {
        if (active) return;

        active = true;
        StartCoroutine(SpawnWaves());
        InvokeRepeating(nameof(CheckAllEnemiesDead), checkInterval, checkInterval);
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < maxWaves)
        {
            currentWave++;
            Debug.Log($"WaveManager: Spawning Wave {currentWave}");

            foreach (EnemySpawner spawner in spawners)
            {
                spawner.ResetSpawner();
                spawner.SpawnEnemies();
            }

            yield return new WaitForSeconds(waveInterval);
        }

        Debug.Log("WaveManager: All waves spawned");
    }

    void CheckAllEnemiesDead()
    {
        int enemyCount = Physics.OverlapSphere(transform.position, checkRadius, enemyLayer).Length;

        if (enemyCount == 0)
        {
            Debug.Log("WaveManager: All enemies defeated. Destroying door.");
            CancelInvoke(nameof(CheckAllEnemiesDead));
            if (objectToDestroy != null) //EW: Null check added to allow script without having something to destroy.
            {
                Destroy(objectToDestroy, destroyDelay);
            }
            
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}