using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WeaponClass : MonoBehaviour
{
    // The level of the weapon
    public int level;
    public AmmoManager ammoManager;
    public float fireRate;
    public GameObject gunShot;
    public GameObject projectilePrefab;
    public float lastFired = Mathf.NegativeInfinity;

    public virtual void Shoot() //Default is spawn projectile
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
    } //"Virtual" allows children to override it
    public virtual void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            ammoManager.ReloadWeapon();
        }
    }

    public virtual void SpawnProjectile() { }

}
