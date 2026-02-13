using UnityEngine;
using System.Collections.Generic;
public class CheckpointDoorScript : MonoBehaviour
{
    [SerializeField] private bool doorsClosed;

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
