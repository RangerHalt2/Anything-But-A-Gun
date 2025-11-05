using Unity.VisualScripting;
using UnityEngine;

public class PlayerPlacement : MonoBehaviour
{
    private PlayerController controller;
    [SerializeField] private Vector3 spawnCoords;

    private void Start()
    {
        controller = GameObject.FindAnyObjectByType<PlayerController>();
        if(controller != null)
        {
            GameObject player = controller.gameObject;
            player.transform.position = spawnCoords;
        }
    }
}
