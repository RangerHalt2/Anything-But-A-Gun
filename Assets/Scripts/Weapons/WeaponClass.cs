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

    public virtual void Shoot()
    { } //"Virtual" allows children to override it
    public virtual void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            ammoManager.ReloadWeapon();
        }
    }
}
