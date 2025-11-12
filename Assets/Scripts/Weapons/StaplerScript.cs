using System.Threading;
using UnityEngine;

public class StaplerScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    [SerializeField] private Hitscan hitscan;
    private float lastFired = Mathf.NegativeInfinity;

    //Logan: This needs a prefab, ryan has a test one and I also made a test one, it allows us to spawn a noise basically and have it play
    //       Cooldown is there so it doesn't spam it per tick.
    private float clickCooldown = 0.5f;
    private float clickTimer = 0;
    [SerializeField] private GameObject clickEffect;
    [SerializeField] private AudioClip gunShot;

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
                        if (hitscan != null)
                        {
                            hitscan.Shoot();
                            // Play sound effect (added by Aaron)
                            SoundEffectsManager.instance.PlaySoundEffectClip(gunShot, transform, 1f);
                            // Old sound effect player (commented out by Aaron)
                            /*if(gunShot != null)
                            {
                                Instantiate(gunShot, transform.position, transform.rotation, null);
                            }*/
                        }
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
                }
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
    }
}
