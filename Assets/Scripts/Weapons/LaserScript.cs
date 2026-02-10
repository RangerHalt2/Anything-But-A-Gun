using System;
using System.Threading;
using UnityEngine;

public class LaserScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    [SerializeField] private Hitscan hitscan;
    
    private float putAway;
    //private float lastFired = Mathf.NegativeInfinity;

    [SerializeField] private float currentHeat;
    private float overheatAmount = 10f;
    private float heatRate = 3f;
    private float coolRate = 1f;
    private bool overheated = false;

    //Logan: This needs a prefab, ryan has a test one and I also made a test one, it allows us to spawn a noise basically and have it play
    //       Cooldown is there so it doesn't spam it per tick.
    //private float clickCooldown = 0.5f;
    //private float clickTimer = 0;
    //[SerializeField] private GameObject gunShot;

    private float timer = 0f;

    public override void Shoot()
    {
        if (!overheated)
        {
            currentHeat += heatRate * Time.deltaTime;
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
                        if (hitscan != null)
                        {
                            if (hasPackAPunch)
                            {
                                Type type = components[currPackAPunchIndex];
                                if(type == typeof(P_Climactic))
                                {
                                    P_Climactic comp = gameObject.GetComponent<P_Climactic>();
                                    float hitScanDmgMod = comp.HitScanDMGCalculation(ammoManager); //Going to connect this to a P_Climatic public function
                                    hitscan.externalDmgMod = hitScanDmgMod;
                                }
                            }
                            hitscan.Shoot();
                            if (gunShot != null)
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
    }

    /*public void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1) 
        {
            ammoManager.ReloadWeapon();
        }
    }*/

    private void Update()
    {
        clickTimer -= Time.deltaTime;
        if (currentHeat >= 0)
        {
            currentHeat -= Time.deltaTime * coolRate;
        }
        if (!overheated && currentHeat > overheatAmount)
        { 
            overheated = true;
        }
        if (overheated && currentHeat <= 0) 
        {
            overheated = false;
        }

    }

    private void OnDisable() 
    {
        putAway = Time.time;
    }
    private void OnEnable()
    {
        currentHeat -= (Time.time - putAway) * coolRate;
    }

}
