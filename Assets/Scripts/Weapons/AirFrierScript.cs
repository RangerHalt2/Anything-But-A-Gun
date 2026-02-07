using System;
using System.Linq;
using UnityEngine;

public class AirFrierScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    //[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    //private float lastFired = Mathf.NegativeInfinity;

    private WeaponLevel weaponLevelRef;
    
    [SerializeField] private float cookLimit1 = 150f; //If cookTime reaches this, and the player fires, only one nugget will spawn
    [SerializeField] private float cookLimit2 = 225f; //If cookTime reaches this, and the player fires, only two nuggets will spawn
    [SerializeField] private float maxCookTime = 300f; //Maximum time before the nuggets are forced to fire. If the player reaches this, three nuggets will spawn
    [SerializeField] private float cookTime = 0f; //Current time the nugget has been cooking
    private float cookSpeed = 0.5f; //Temperature of the oven, ha ha
    [SerializeField] private float coolSpeed = 0.1f; //Speed that the nuggies cool off
    [SerializeField] private InputManager IM;

    private PlayerController playerRef;

    //NEEDS UI FOR CHARGE TIME

    private void Awake()
    {
        cookTime = 0f;
        weaponLevelRef = GetComponent<WeaponLevel>();
        playerRef = GameObject.FindAnyObjectByType<PlayerController>();
        IM = GameObject.FindFirstObjectByType<InputManager>();

        components = acceptablePackAPunchOptions.Select(t => t.GetTypeSafe()).ToArray();
    }

    private void Update() 
    {
        if (cookTime >= 0)
        {
            cookTime -= coolSpeed;
        }
        else 
        {
            cookTime = 0;
        }

        if (!IM.FireInput && cookTime > cookLimit1)
        {
            // If there is an assigned ammo manager, and that ammo manager has at least one round of ammo loaded
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
            {
                // Attempt to fire the weapon
                ammoManager.Fire();
                // If the weapon is not reloading
                if (!ammoManager.IsReloading())
                {
                    if (projectilePrefab != null)
                    {
                        SpawnProjectile();
                        cookTime = 0f;
                        Instantiate(gunShot, transform.position, transform.rotation, null);
                    }
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
                }

            }
        }

    }
    
    public override void Shoot()
    {
       cookTime += cookSpeed;
    }

    /*public void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1) 
        {
            ammoManager.ReloadWeapon();
        }
    }*/

    public override void SpawnProjectile()
    {
        int nuggies = 0; //number of nuggies that spawn
        if (cookTime >= maxCookTime) { nuggies = 3; }
        else if (cookTime >= cookLimit2) { nuggies = 2; }
        else if (cookTime >= cookLimit1) { nuggies = 1; }
        Debug.Log(nuggies);

        if (hasPackAPunch)
        {
            Type type = components[currPackAPunchIndex];
            if (type == typeof(P_FridayFunday))
                nuggies *= 2; //2 times the amount of nuggies
        }

        while (nuggies > 0)
        {
            nuggies--;
            // Check that the prefab is valid
            if (projectilePrefab != null)
            {
                // Create the projectile
                GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

                // Account for spread
                Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
                projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);

                // Keep the heirarchy organized
                if (projectileSpawnPoint == null && GameObject.Find("ProjectileSpawnPoint") != null)
                {
                    projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;

                }

                Projectile proj = projectileGameObject.GetComponent<Projectile>();
                proj.SetWeaponLevelReference(weaponLevelRef);

                NuggieSpawner ns = projectileGameObject.GetComponent<NuggieSpawner>();
                ns.SetWeaponLevelReference(weaponLevelRef);
            }
        }
    }
}
