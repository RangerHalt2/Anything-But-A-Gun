using UnityEngine;

public class ShurikenWeaponScript : WeaponClass
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
                            // Play sound effect (added by Aaron)
                            if (gunShot != null)
                            Instantiate(gunShot, transform.position, transform.rotation, null);
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
                    // Play sound effect (added by Aaron)
                    if (gunShot != null)
                    Instantiate(gunShot, transform.position, transform.rotation, null);
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

            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);

            // Keep the heirarchy organized
            if (projectileSpawnPoint == null && GameObject.Find("ProjectileSpawnPoint") != null)
            {
                projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;

            }

            if (hasPackAPunch)
            {
                Component comp = projectileGameObject.gameObject.AddComponent(components[currPackAPunchIndex]);
            }

            Projectile shiProj = projectileGameObject.GetComponent<Projectile>();
            shiProj.SetWeaponLevelReference(weaponLevelRef);
        }
    }
}
