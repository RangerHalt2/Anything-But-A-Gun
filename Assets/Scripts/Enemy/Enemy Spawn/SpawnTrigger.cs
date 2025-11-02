//Author and Contributor: Logan Baysinger
//Purpose: Handles the triggering of the EnemySpawner.cs, just a helper code.
//Use: Assign the enemy spawner it should be calling the spawn enemies function of and set the collider to trigger.

using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            if (enemySpawner != null) enemySpawner.SpawnEnemies();
        }
    }
}
