using System;
using Unity.VisualScripting;
using UnityEngine;

public class DivorcePaper : WeaponClass
{
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private Transform projectileSpawnPoint;
    private float timer = 0;


    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
    }

    
    public override void Shoot()
    {

        if (timer < 0)
        {
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
            {
                if (!ammoManager.IsReloading())
                {
                    ammoManager.Fire();
                    // Play sound effect (added by Aaron)
                    if(gunShot != null)
                        Instantiate(gunShot, transform.position, transform.rotation, null);

                    if (hasPackAPunch)
                    {
                        Type addedType = components[currPackAPunchIndex];
                        if(addedType == typeof(P_FridayFunday))
                            SpawnProjectile(projectilePrefab); //Spawn an extra projectile if we have Friday Funday.
                    }
                    SpawnProjectile(projectilePrefab);
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

    private void SpawnProjectile(GameObject bullet)
    {
        if (bullet == null)
        {
            Debug.LogError("The Bullet for the VolleyBall was null, a tech likely forgot to assemble the weapon properly!");
            return;
        }

        GameObject projectile = Instantiate(bullet, projectileSpawnPoint.position, projectileSpawnPoint.rotation, null);
        //Instantiate(gunShot, transform.position, transform.rotation, null);

        if(projectile != null && meshes.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, meshes.Length);
            MeshFilter currentMesh = projectile.GetComponentInChildren<MeshFilter>();
            currentMesh.mesh = meshes[index];
        }

        Vector3 rotationEulerAngles = projectile.transform.rotation.eulerAngles;
        projectile.transform.rotation = Quaternion.Euler(rotationEulerAngles);
        if (hasPackAPunch)
        {
            Component comp = projectile.gameObject.AddComponent(components[currPackAPunchIndex]);
            Type addedType = comp.GetType();
            if (addedType == typeof(P_Climactic))
            {
                P_Climactic climatic = comp.gameObject.GetComponent<P_Climactic>();
                climatic.parentWeapon = gameObject.GetComponent<WeaponClass>();
            }
        }
        Projectile proj = projectile.GetComponent<Projectile>();
        proj.SetWeaponLevelReference(weaponLevel);
        PlayOnomatopeia();
        RandomGunShot(proj.transform);
    }

    private new void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
    }

}
