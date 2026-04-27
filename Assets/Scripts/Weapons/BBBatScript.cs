using UnityEngine;
using System.Collections;

public class BBBatScript : WeaponClass
{

    /*//[SerializeField] public int level {get; set;}

    //[SerializeField] private int health = 10; //Health of the melee weapon
    [SerializeField] private int teamID;
    private int health = 10;
    private bool striking = false;
    //[SerializeField] private float baseDamage = 15f;
    [SerializeField] private float growthRate = 1.15f;
    //private float cummulativeDamage;
    //private bool striking = false;
    private Health enemyHealth;
    //private AmmoManager ammoManager;
    //private WeaponLevel currentWeaponLevel;
    //[SerializeField] private GameObject whackEffect;
    private WeaponCollectScript wcs;
    private WeaponHandler wh;
    private Projectile proj;
    private Collider col;

    [Tooltip("This is a sort of back-end buffer time to how frequently the player can hit the enemy with the baseball bat")]
    [SerializeField] private float attackCooldownBuffer = 0.5f;
    private float attackTimer = 0;
    private bool throwing = false;

    private void Start()
    {
        //proj = GetComponent<Projectile>();
        wcs = GetComponent<WeaponCollectScript>();
        wh = GetComponentInParent<WeaponHandler>();
        ammoManager = GetComponent<AmmoManager>();
        weaponLevel = GetComponent<WeaponLevel>();
        col = GetComponent<Collider>();
        if(weaponLevel != null)
            UpdateLevelDamage();
    }
    //This is me trying to make a new BBBat script working.
    /*private void Update() 
    {
        if (wcs.collected)
        {
            col.enabled = false;
        }
        else if (!throwing)
        {
            col.enabled = true;
        }
    }

    public override void Shoot()
    {
        if (!throwing) 
        {
            throwing = true;
            ThorsMight(); 
        }
    }

    void ThorsMight() 
    {
        wh.DropWeapon(gameObject);
        proj.enabled = true;
    }

    void Update()
    {
        if (health <= 0)
        {
            //Change visual to Broken BBBat
        }
        attackTimer -= Time.deltaTime;
    }

    void OnTriggerStay(Collider _other) //Detect if an enemy is near
    {
        enemyHealth = _other.GetComponentInParent<Health>();

            if (enemyHealth != null && enemyHealth.teamID != teamID && striking && attackTimer <= 0 && ammoManager.GetCurrentAmmo() > 0) // If it's an enemy, the bat has health, and the player presses shoot, attack the enemy
        {
            //_other.gameObject.GetComponent<EnemyController>().LoseLife(); Make enemy/damageable lose life
            UpdateLevelDamage(); //Confirm the level damage before DOING damage.
            enemyHealth.TakeDamage(levelDamage);
            health--;
            ammoManager.Fire();
            if(gunShot != null)
            {
                Instantiate(gunShot, transform.position, transform.rotation, null);
            }
            attackTimer = attackCooldownBuffer;
        }
    }

    public override void Shoot() //Required method for interface IWeapon
    {
        if (!striking)
        {
            StartCoroutine(CLASH());
            //Animate Attack
        }
    }

    IEnumerator CLASH()
    {
        striking = true;
        yield return new WaitForSeconds(2);
        striking = false;
    }

    /*public void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1) 
        {
            ammoManager.ReloadWeapon();
        }
    }

    public void UpdateLevelDamage()
    {
        levelDamage = baseDamage * Mathf.Pow(growthRate, weaponLevel.Level);
    }

}*/

    public bool launched;
    [SerializeField] private Transform projectileSpawnPoint;
    public MeshRenderer mesh; //For turning off
    private WeaponLevel weaponLevelRef;
    private float cummulativeDamage;
    private WeaponLevel currentWeaponLevel;
    [SerializeField] private float growthRate;
    private float timer = 0;


    private void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
        mesh = GetComponentInChildren<MeshRenderer>();
    }

    public override void Shoot() //Default is spawn projectile
    {
        if (!launched)
        {
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

                        if (projectilePrefab != null)
                        {
                            SpawnProjectile();
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
    } //"Virtual" allows children to override it

    private new void Update()
    {
        base.Update();
        timer -= Time.deltaTime;
    }


    public override void SpawnProjectile()
    {
        //Disable the visible mesh.
        mesh.enabled = false;

        //Spawn the projectile
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            launched = true;

            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.baseDamage = baseDamage;
            proj.SetWeaponLevelReference(weaponLevelRef);
            PlayOnomatopeia();

            if (hasPackAPunch)
            {
                proj.gameObject.AddComponent(components[currPackAPunchIndex]);
            }

            projectileGameObject.GetComponent<BBBatProjectileScript>().baseWeapon = gameObject;
        }
    }
}
