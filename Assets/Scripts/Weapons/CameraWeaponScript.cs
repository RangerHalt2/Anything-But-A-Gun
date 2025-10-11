using UnityEngine;

public class CameraWeaponScript : MonoBehaviour, IWeapon
{
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    private float lastFired = Mathf.NegativeInfinity;

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
        //Make this camera based? Or AOE?
    }
}
