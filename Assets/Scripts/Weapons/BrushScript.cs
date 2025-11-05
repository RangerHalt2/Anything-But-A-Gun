using UnityEngine;

public class BrushScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}
    
    //[SerializeField] private float fireRate = 0.25f;
    //[SerializeField] private AmmoManager ammoManager;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Transform projectileSpawnPoint;
    private float lastFired = Mathf.NegativeInfinity;

    private PlayerController playerRef;

    private WeaponLevel weaponLevelRef;

    private bool leftSlash = true; //Which direction the slash is bending.
    private float slashAngle = 0f; //Just the angle that the slash bends to
    private float slashAngleLeft = -26f;
    private float slashAngleRight = 21f;
    [SerializeField] private GameObject gunShot;
    [SerializeField] private GameObject gunShot2;
    [SerializeField] private Transform gunShotTrans;

    private void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
        playerRef = GameObject.FindAnyObjectByType<PlayerController>();
    }

    public override void Shoot()
    {
        // If enough time has passed since the last round was fired
        if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
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
                        if (leftSlash)
                        {
                            slashAngle = slashAngleRight;
                            leftSlash = false;
                            if (gunShot !=null)
                            {
                                Instantiate(gunShot, gunShotTrans.position, transform.rotation, null);
                            }

                        }
                        else
                        {
                            slashAngle = slashAngleLeft;
                            leftSlash = true;
                            if (gunShot !=null)
                            {
                                Instantiate(gunShot2, gunShotTrans.position, transform.rotation, null);
                            }

                        }

                        SpawnProjectile();
                    }
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
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

    void SpawnProjectile()
    {

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
                Physics.IgnoreCollision(proj.GetComponent<CapsuleCollider>(), playerRef.GetComponent<CharacterController>(), true);
            }
        }
    }
}
