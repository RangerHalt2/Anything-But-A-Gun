using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BrushScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}
    
    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    //[SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Transform projectileSpawnPoint;
    //private float lastFired = Mathf.NegativeInfinity;

    private PlayerController playerRef;

    private WeaponLevel weaponLevelRef;

    private bool leftSlash = true; //Which direction the slash is bending.
    private float slashAngle = 0f; //Just the angle that the slash bends to
    private float slashAngleLeft = -26f;
    private float slashAngleRight = 21f;
    //[SerializeField] private GameObject gunShot;
    [SerializeField] private GameObject gunShot2;
    [SerializeField] private Transform gunShotTrans;
    [SerializeField] private AudioClip brushSFX;

    private float timer = 0f;

    private void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
        playerRef = GameObject.FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public override void Shoot()
    {
        // If enough time has passed since the last round was fired
        if (timer <= 0)
        {
            Debug.Log("Shooting weapon");
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
                        // Need a null check here if you wanna add in something that might be null. This is why it wasn't firing
                        if (brushSFX != null)
                        {
                            // Play sound effect (added by Aaron)
                            SoundEffectsManager.instance.PlaySoundEffectClip(brushSFX, transform, 1f);
                        }
                        if (hasPackAPunch)
                        {
                            if (components[currPackAPunchIndex] == typeof(P_FridayFunday)) //If it has the friday funday pack a punch then shoot twice.
                            {
                                SpawnProjectile();
                            }
                        }
                        SpawnProjectile();
                    }
                    // Update lastFired
                    timer = fireRate;
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

    public override void SpawnProjectile()
    {

        if (leftSlash)
        {
            slashAngle = slashAngleRight;
            leftSlash = false;
            if (gunShot != null)
            {
                Instantiate(gunShot, gunShotTrans.position, transform.rotation, null);
            }

        }
        else
        {
            slashAngle = slashAngleLeft;
            leftSlash = true;
            if (gunShot != null)
            {
                Instantiate(gunShot2, gunShotTrans.position, transform.rotation, null);
            }

        }

        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles.x, rotationEulerAngles.y, rotationEulerAngles.z + slashAngle);

            // Keep the heirarchy organized
            if (projectileSpawnPoint == null && GameObject.Find("ProjectileSpawnPoint") != null)
            {
                projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;

            }
            Projectile [] projs = projectileGameObject.GetComponentsInChildren<Projectile>();
            foreach (Projectile proj in projs)
            {
                proj.SetWeaponLevelReference(weaponLevelRef);
                RandomGunShot(proj.transform);
                Physics.IgnoreCollision(proj.GetComponent<CapsuleCollider>(), playerRef.GetComponent<CharacterController>(), true);
                if (hasPackAPunch)
                {
                    Component comp = proj.gameObject.AddComponent(components[currPackAPunchIndex]);
                }
            }
        }
    }
}
