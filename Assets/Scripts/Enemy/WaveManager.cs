using UnityEngine;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private List<EnemySpawner> spawners;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float checkRadius = 50f;
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private float checkInterval = 5f;

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
        StartNextWave();
        InvokeRepeating(nameof(CheckEnemiesAlive), checkInterval, checkInterval);
    }

    void StartNextWave()
    {
        if (currentWave >= maxWaves)
        {
            Debug.Log("All waves completed");
            CancelInvoke(nameof(CheckEnemiesAlive));

            Destroy(objectToDestroy, destroyDelay);
            return;
        }

        currentWave++;
        Debug.Log($"Starting Wave {currentWave}");

        foreach (EnemySpawner spawner in spawners)
        {
            spawner.ResetSpawner();
            spawner.SpawnEnemies();
        }
    }

    void CheckEnemiesAlive()
    {
        int enemyCount = Physics.OverlapSphere(transform.position, checkRadius, enemyLayer).Length;

        if (enemyCount == 0)
            StartNextWave();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}