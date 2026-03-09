using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomWaveSpawner : MonoBehaviour
{
    [Tooltip("This bool determines if the waves will go 1 then 2 then 3, or if they're allowed to spawn wave 2 while wave 1 enemies are still around")]
    [SerializeField] bool staggered_waves = false;
    //[SerializeField] private Transform[] spawnPoints;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private int num_of_waves = 3;
    private int current_wave = 0;
    [SerializeField] private float spawnAllBySeconds = 15f;
    
    [Space(2)]
    [Tooltip("An array of all the doors to remove when the wave manager is done")]
    [SerializeField] private Transform[] doors_to_remove;

    private int interval = 0;

    private bool locked_doors = false;

    private void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        interval = (int)(spawnAllBySeconds / (num_of_waves-1));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && current_wave < num_of_waves)
        {
            StartCoroutine(SpawnWaves());
            locked_doors = true;
            Debug.Log("ROOM WAVE SPAWNER - Locking the doors");
            foreach (Transform trans in doors_to_remove)
            {
                trans.gameObject.SetActive(locked_doors);
            }
        }
    }

    private IEnumerator SpawnWaves()
    {
        if (!staggered_waves)
        {
            while (current_wave < num_of_waves)
            {
                enemySpawner.ResetSpawner();
                enemySpawner.SpawnEnemies();
                current_wave++;
                AssignListeners();
                if (current_wave < num_of_waves)
                    yield return new WaitForSeconds(interval);
            }
        }
        else
        {
            if (current_wave < num_of_waves)
            {
                enemySpawner.ResetSpawner();
                enemySpawner.SpawnEnemies();
                current_wave++;
                AssignListeners();
            }
        }
    }

    /*
    private void Update()
    {
        if(current_wave == num_of_waves && doors_to_remove != null && enemySpawner.aliveEnemies.Count <= 0 && locked_doors)
        {
            locked_doors = false;
            Debug.Log("ROOM WAVE SPAWNER - Doors unlocking from Update");
            foreach (Transform trans in doors_to_remove)
            {
                trans.gameObject.SetActive(locked_doors);
            }
        }
    }
    */

    private void AssignListeners()
    {
        Debug.Log("ROOM WAVE SPAWNER - Assigning Listeners");
        foreach(EnemyClass enemy in enemySpawner.enemies)
        {
            enemy.OnEnemyDeath += HandleEnemyDeath;
        }

        Debug.Log("ROOM WAVE SPAWNER - Done Assigning Listeners");
        enemySpawner.enemies.Clear();
    }

    private void HandleEnemyDeath(EnemyClass enemy)
    {
        Debug.Log("ROOM WAVE SPAWNER - Handle Enemy Death Event");
        enemySpawner.aliveEnemies.Remove(enemy);

        if (enemySpawner.aliveEnemies.Count == 0  && current_wave < num_of_waves)
        {
            StartCoroutine(SpawnWaves());
        }

        if(current_wave == num_of_waves && doors_to_remove != null && enemySpawner.aliveEnemies.Count <= 0)
        {
            locked_doors = false;
            Debug.Log("ROOM WAVE SPAWNER - Doors unlocking");
            foreach (Transform trans in doors_to_remove)
            {
                trans.gameObject.SetActive(locked_doors);
            }
        }

    }

}
