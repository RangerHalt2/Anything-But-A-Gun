using UnityEngine;
using System.Collections.Generic;
public class DoorController : MonoBehaviour
{
    private CentralEnemyManager enemyManager;
    [SerializeField] private bool doorsClosed;

    void Start()
    {
        enemyManager = CentralEnemyManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.ActiveEnemyCount() == 0)
        {
            doorsClosed = false;
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
            doorsClosed = true;
            //Turn on the doors
            foreach (Transform child in transform)
            {
                // Set the child GameObject to inactive
                child.gameObject.SetActive(true);
            }
        }
    }
}
