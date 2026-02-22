//Purpose: This script rotates the gun and camera up and down
//Author: Logan Baysinger.

using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] public float sensitivity = 5f;
    [SerializeField] private float maxLookingAngle = -80f;
    [SerializeField] private float minLookingAngle = 80f;
    [SerializeField] private float controllerSens = 7f;

    private InputManager inputs;

    private float rotationX;

    private Health playerHealth;
    private UIManager uiManager;

    private WinEvent winEvent;
    private void Start()
    {
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        playerHealth = GetComponentInParent<Health>();
        winEvent = GameObject.FindAnyObjectByType<WinEvent>();

        sensitivity = LoadSensitivity();
    }

    private float LoadSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity", 0.1f);
        // 1.0f is default if no value exists
    }

    private void Rotate(Vector2 Rotation)
    {
        rotationX += -Rotation.y * sensitivity * (inputs.ControllerLast ? controllerSens : 1f);
        rotationX = Mathf.Clamp(rotationX, minLookingAngle, maxLookingAngle);
        transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void Update()
    {
        if (playerHealth.isDead) return;

        if (winEvent != null  && winEvent.hasWon) return;

        if (UIManager.instance != null && UIManager.instance.IsPaused) return;
        
        Rotate(inputs.LookInput);
    }

}
