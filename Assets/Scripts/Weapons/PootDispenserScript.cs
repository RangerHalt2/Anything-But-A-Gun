using UnityEngine;

public class PootDispenserScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    //[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    //private float lastFired = Mathf.NegativeInfinity;

    private WeaponLevel weaponLevelRef;

    private void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
    }

    override
    public void Shoot()
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
                        }
                        // Update lastFired
                        lastFired = Time.timeSinceLevelLoad;

                    }
                }
       
        }

    }

    override
    public void Reload()
    {
        // If the shooter has at least one round of reserve ammo or is set to have infinite ammo
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            // Reload the shooter
            ammoManager.ReloadWeapon();
        }
    }

    override
    public void SpawnProjectile()
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
            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.SetWeaponLevelReference(weaponLevelRef);

            PootProjectileScript pootProj = projectileGameObject.GetComponent<PootProjectileScript>();
            pootProj.SetWeaponLevelReference(weaponLevelRef);
        }
    }
}
