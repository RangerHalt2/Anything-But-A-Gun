// Created By: Ryan Lupoli
// Detects what the player is currently looking at
using UnityEngine;

public class LookAtDetector : MonoBehaviour
{
    [Tooltip("How close the player needs to be from an object to be considered to be looking at it.")]
    [SerializeField] private float maxLookDistance = 5f;
    [Tooltip("The layer used by objects which can be interacted with.")]
    [SerializeField] private LayerMask interactableLayer;

    private InputManager inputs;

    private WeaponClass currentWeapon;

    // Update is called once per frame
    void Update()
    {
        // Shoot a ray cast to see if player is looking at something
        Ray ray = new Ray(transform.position, transform.forward);
        UIManager uiManager = GameObject.FindAnyObjectByType<UIManager>();
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        if (inputs != null && inputs.MidGameUIView)
        {
            Debug.Log("LOOK AT DETECTOR - preventing a second ui from showing up.");
            ClearWeapon();
            return;
        }
        if (uiManager != null && uiManager.isPaused)
        {
            Debug.Log("LOOK AT DETECTOR - Game is paused preventing the ray from firing");
            ClearWeapon();
            return;
        }
        if (Physics.Raycast(ray, out RaycastHit hit, maxLookDistance, interactableLayer))
        {
            // Check if the player is looking at a weapon
            WeaponClass weapon = hit.collider.GetComponent<WeaponClass>();
            WeaponCollectScript collectScript = hit.collider.GetComponent<WeaponCollectScript>();
            if (weapon != null)
            {
                // Skip showing panel if weapon has been collected
                if (collectScript != null && collectScript.collected)
                {
                    ClearWeapon();
                    return;
                }

                if (weapon != currentWeapon)
                {
                    if (currentWeapon != null)
                    {
                        currentWeapon.HideInfo();
                    }
                    currentWeapon = weapon;
                    currentWeapon.ShowInfo();
                }
            }
            else
            {
                ClearWeapon();
            }
        }
        else
        {
            ClearWeapon();
        }
    }

    private void ClearWeapon()
    {
        // If a current weapon is assigned
        if (currentWeapon != null)
        {
            // Hide its info
            currentWeapon.HideInfo();
            // Set current weapon to null
            currentWeapon = null;
        }
    }
}
