using UnityEngine;
using System.Collections.Generic;
public class DoorController : MonoBehaviour
{
    [SerializeField] private CentralEnemyManager enemyManager;

    void Start()
    {
        enemyManager = CentralEnemyManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.ActiveEnemyCount() == 0)
        {
            //Doors will be turned off
            foreach (Transform child in transform)
            {
                // Set the child GameObject to inactive
                child.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider _other) 
    {
        if (_other.CompareTag("Player")) 
        {
            //Turn on the doors
            foreach (Transform child in transform)
            {
                // Set the child GameObject to inactive
                child.gameObject.SetActive(true);
            }
        }
    }
}
