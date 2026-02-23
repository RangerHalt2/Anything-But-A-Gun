using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FinalEntDoor : MonoBehaviour
{
    public GameObject entranceDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (entranceDoor != null)
            {
                entranceDoor.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }
}
