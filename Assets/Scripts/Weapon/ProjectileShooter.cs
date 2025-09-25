// Created By Ryan Lupoli
// This is a projectile shooter intended for testing purposes. I know this script is a mess, it will be refined or replaced later
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab = null;

    public AmmoManager ammoManager;

    [Header("Firing Settings")]
    [Tooltip("The minimum time between projectiles being fired.")]
    public float fireRate = 0.05f;

    public InputAction fireAction;

    public InputAction reloadAction;

    [Tooltip("The transform in the heirarchy which holds projectiles if any")]
    public Transform projectileHolder = null;

    private float lastFired = Mathf.NegativeInfinity;


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
        if (fireAction.ReadValue<float>() >= 1)
        {
            Fire();
        }
        if (reloadAction.ReadValue<float>() >= 1)
        {
            if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
            {
                ammoManager.ReloadWeapon();
            }
            
        }
    }

    void Fire()
    {
        if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
        {
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
            {
                // Launches a projectile
                SpawnProjectile();

                lastFired = Time.timeSinceLevelLoad;

                ammoManager.Fire();
            }
        }
    }
    

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
