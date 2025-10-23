using UnityEngine;
using System.Collections.Generic;

public class CentralEnemyManager : MonoBehaviour
{
    public static CentralEnemyManager Instance { get; private set; }
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public GameObject FindClosestEnemy(Vector3 pointOfOrigin)
    {
        GameObject closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy == null) continue; // Skip if enemy is destroyed.
            Vector3 directionToEnemy = enemy.transform.position - pointOfOrigin;
            float distanceSqr = directionToEnemy.sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }
        
        return closestEnemy;
        
    }

    public int ActiveEnemyCount()
    {
        return activeEnemies.Count;
    }
}

