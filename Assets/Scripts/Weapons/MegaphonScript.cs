using UnityEngine;

public class MegaphonScript : MonoBehaviour, IWeapon
{
    [SerializeField] public int level {get; set;}

    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    private float lastFired = Mathf.NegativeInfinity;

    // Damage Settings
    [SerializeField] public float baseDamage { get; set; }
    [SerializeField] public float levelDamage { get; set; }
    [HideInInspector] public float cumulativeDamage { get;  set; }

    private WeaponLevel weaponLevelRef;
    private int currentWeaponLevel;

    private void Start()
    {
        // Look for a Weapon Level Component
        weaponLevelRef = GetComponent<WeaponLevel>();
        // If the component was found...
        if (weaponLevelRef != null)
        {
            // Update Current Weapon Level
            currentWeaponLevel = weaponLevelRef.Level;
        }
        // If no level component was found...
        else
        {
            // Set Weapon Level to 0
            currentWeaponLevel = 0;
        }
        UpdateLevelDamage();
    }
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

    public void Reload()
    {
        // If the shooter has at least one round of reserve ammo or is set to have infinite ammo
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            // Reload the shooter
            ammoManager.ReloadWeapon();
        }
    }

    void SpawnProjectile()
    {
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.SetWeaponLevelReference(weaponLevelRef);

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

    #region Damage & Level Code
    // Update the Level Damage for the weapon
    public void UpdateLevelDamage()
    {
        cumulativeDamage = baseDamage + (levelDamage * (currentWeaponLevel - 1));
    }

    public float GetWeaponDamage()
    {
        return cumulativeDamage;
    }
    #endregion
}

