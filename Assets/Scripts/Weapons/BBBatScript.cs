using UnityEngine;
using System.Collections;

public class BBBatScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

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
    }*/

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
    }*/

    public void UpdateLevelDamage()
    {
        levelDamage = baseDamage * Mathf.Pow(growthRate, weaponLevel.Level);
    }

}
