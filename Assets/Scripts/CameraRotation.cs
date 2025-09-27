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

    private void Start()
    {
        inputs = GameObject.FindAnyObjectByType<InputManager>();
    }

    private void Rotate(Vector2 Rotation)
    {
        rotationX += -Rotation.y * sensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, minLookingAngle, maxLookingAngle);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void Update()
    {
        Rotate(inputs.LookInput);
    }

}
