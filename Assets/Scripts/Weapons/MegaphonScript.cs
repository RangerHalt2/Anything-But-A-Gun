using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class MegaphonScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    //[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    //private float lastFired = Mathf.NegativeInfinity;

    private WeaponLevel weaponLevelRef;

    private float timer = 0f;

    private void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
    }

    public override void Shoot()
    {
        // If enough time has passed since the last round was fired
        if (timer <= 0)
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
                        if (gunShot !=null)
                        {
                            Instantiate(gunShot, transform.position, transform.rotation, null);
                        }
                    }
                    // Update lastFired
                    timer = fireRate;
                }
            }
            else if (ammoManager != null)
            {
                if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
                {
                    ammoManager.ReloadWeapon();
                }
                else
                {
                    if (clickEffect != null && clickTimer <= 0)
                    {
                        clickTimer = clickCooldown;
                        Instantiate(clickEffect, transform.position, transform.rotation, null);
                    }
                }
            }
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
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
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.SetWeaponLevelReference(weaponLevelRef);
            RandomGunShot(proj.transform);
            
            if (hasPackAPunch)
            {
                Component comp = proj.gameObject.AddComponent(components[currPackAPunchIndex]);
                Type addedType = comp.GetType();
                if (addedType == typeof(P_Climactic))
                {
                    P_Climactic climatic = comp.gameObject.GetComponent<P_Climactic>();
                    climatic.parentWeapon = gameObject.GetComponent<WeaponClass>();
                }
            }


            /*
            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);
            */

            /*
            // Keep the heirarchy organized
            if (projectileSpawnPoint == null && GameObject.Find("ProjectileSpawnPoint") != null)
            {
                projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;

            }
            */
        }
    }
}

