//Purpose: This script rotates the gun and camera up and down
//Author: Logan Baysinger.

using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float maxLookingAngle = -80f;
    [SerializeField] private float minLookingAngle = 80f;

    private InputManager inputs;

    private float rotationX;

    private Health playerHealth;

    private WinEvent winEvent;
    private void Start()
    {
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        playerHealth = GetComponentInParent<Health>();
        winEvent = GameObject.FindAnyObjectByType<WinEvent>();
    }

    private void Rotate(Vector2 Rotation)
    {
        rotationX += -Rotation.y * sensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, minLookingAngle, maxLookingAngle);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void Update()
    {
        if(playerHealth.isDead || winEvent.hasWon) return;
        Rotate(inputs.LookInput);
    }

}
