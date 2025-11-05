using UnityEngine;
using System.Collections;

public class BBBatScript : WeaponClass
{
    //[SerializeField] public int level {get; set;}

    [SerializeField] private int health = 10; //Health of the melee weapon
    [SerializeField] private int teamID;
    [SerializeField] private float baseDamage = 15f;
    [SerializeField] private float growthRate = 1.15f;
    private float cummulativeDamage;
    private bool striking = false;
    private Health enemyHealth;
    //private AmmoManager ammoManager;
    private WeaponLevel currentWeaponLevel;
    [SerializeField] private GameObject whackEffect;

    [Tooltip("This is a sort of back-end buffer time to how frequently the player can hit the enemy with the baseball bat")]
    [SerializeField] private float attackCooldownBuffer = 0.5f;
    private float attackTimer = 0;

    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
        currentWeaponLevel = GetComponent<WeaponLevel>();
        if(currentWeaponLevel != null)
            UpdateLevelDamage();
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
            enemyHealth.TakeDamage(cummulativeDamage);
            health--;
            ammoManager.Fire();
            if(whackEffect != null)
            {
                Instantiate(whackEffect, transform.position, transform.rotation, null);
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
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
    }

}
