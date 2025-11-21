using UnityEngine;

public class DVDPlayer : WeaponClass
{

    [Tooltip("Transform where the weapon's projectiles are spawned.")]
    [SerializeField] private Transform projectileSpawnPoint;

    private WeaponLevel weaponLevelRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the reference to the Weapon's Level
        weaponLevelRef = GetComponent<WeaponLevel>();
    }

    public override void Shoot()
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
                    if (projectilePrefab != null)
                    {
                        SpawnProjectile();
                        if (gunShot !=null)
                        {
                            Instantiate(gunShot, transform.position, transform.rotation, null);
                        }
                    }
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
                }
            }
        }
    }

    public override void SpawnProjectile()
    {
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.SetWeaponLevelReference(weaponLevelRef);
        }
    }
}
