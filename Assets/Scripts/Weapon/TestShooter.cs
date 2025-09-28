// Created By: Ryan Lupoli
// This is script is intended to help test the firing process of projectile and hitscan based weapons
// This is not intended for use in the final game

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShooter : MonoBehaviour
{

    #region Variables
    [Header("Firing Mode")]
    public FiringMode firingMode;
    public enum FiringMode
    {
        Projectile,
        Hitscan
    }

    [Header("Firing Rate Settings")]
    [Tooltip("How often (in seconds) the weapon is able to fire.")]
    [SerializeField] private float fireRate = 0.25f;
    // The time the weapon was last fired
    private float lastFired = Mathf.NegativeInfinity;

    [Header("Input Settings")]
    [Tooltip("The action used to fire the testing shooter.")]
    [SerializeField] private InputAction fireAction;
    [Tooltip("The action used to reload the testing shooter.")]
    [SerializeField] private InputAction reloadAction;

    [Header("References")]
    [Tooltip("Reference to the game object with the Ammo Manager for this shooter")]
    [SerializeField] private AmmoManager ammoManager;
    [Tooltip("Reference to the game object with the Hitscan script (Used if in Hitscan Mode)")]
    [SerializeField] private Hitscan hitscan;
    [Tooltip("Reference to the prefab used for projectiles (Used if in Projectiles Mode)")]
    [SerializeField] private GameObject projectilePrefab;
    [Tooltip("Optional: Refernce to the game object any spawned projectiles will be made children of")]
    [SerializeField] private Transform projectileHolder;

    #endregion

    void OnEnable()
    {
        fireAction.Enable();
        reloadAction.Enable();
    }

    void OnDisable()
    {
        fireAction.Disable();
        reloadAction.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        // Fire Action Performed
        if (fireAction.ReadValue<float>() >= 1)
        {
            // If enough time has passed since the last round was fired
            if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
            {
                // If there is an assigned ammo manager, and that ammo manager has at least one round of ammo loaded
                if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
                {
                    // Attempt to fire the weapon
                    ammoManager.Fire();
                    // If the weapon is not reloading
                    if (!ammoManager.IsReloading())
                    {
                        // If the testShooter is configured to fire projectiles
                        if (firingMode == FiringMode.Projectile)
                        {
                            if (projectilePrefab != null)
                            {
                                SpawnProjectile();
                            }
                        }
                        // If the testShooteris configured to use hitscan
                        else if (firingMode == FiringMode.Hitscan)
                        {
                            if (hitscan != null)
                            {
                                hitscan.Shoot();
                            }
                        }
                        // Update lastFired
                        lastFired = Time.timeSinceLevelLoad;
                    }
                }
            }
        }

        // Reload Action Performed
        if (reloadAction.ReadValue<float>() >= 1)
        {
            // If the shooter has at least one round of reserve ammo or is set to have infinite ammo
            if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
            {
                // Reload the shooter
                ammoManager.ReloadWeapon();
            }

        }
    }
    
    // Spawns a projectile prefab
    public void SpawnProjectile()
    {
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, transform.position, transform.rotation, null);

            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);

            // Keep the heirarchy organized
            if (projectileHolder == null && GameObject.Find("ProjectileHolder") != null)
            {
                projectileHolder = GameObject.Find("ProjectileHolder").transform;
            }
            if (projectileHolder != null)
            {
                projectileGameObject.transform.SetParent(projectileHolder);
            }
        }
    }
}
