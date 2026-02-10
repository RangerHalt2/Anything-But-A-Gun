using System.Collections;
using UnityEngine;

public class JoustingHorseScript : WeaponClass
{
    //[SerializeField] private GameObject joustingCollider;
    private Rigidbody joustingRB;
    public Collider joustingCollider;
    private JoustingColliderScript jCS;
    [SerializeField] private float joustSpeedMultiplier; //How fast you joust
    [SerializeField] private float joustTime; //How long a joust lasts
    private float currentDamage;
    [SerializeField] private PlayerController pc;
    private float pcBaseSpeed;
    public bool JOUSTING = false;

    private float timer = 0f;

    private WeaponLevel weaponLevelRef;

    void Start()
    {
        weaponLevelRef = GetComponent<WeaponLevel>();
        joustingRB = GetComponentInChildren<Rigidbody>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        pcBaseSpeed = pc.movementSpeed;
        jCS = GetComponentInChildren<JoustingColliderScript>();
        jCS.myTeamID = pc.gameObject.GetComponent<Health>().teamID;
        jCS.damage = baseDamage;
    }

    public override void Shoot()
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

                    if (hasPackAPunch)
                    {
                        if (components[currPackAPunchIndex] == typeof(P_ShiningArmor))
                        {
                            P_ShiningArmor self = gameObject.GetComponent<P_ShiningArmor>();
                            self.ApplyArmor();
                        }
                    }

                    StartCoroutine(JOUST());
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

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnDisable()
    {
        StopCoroutine(JOUST());
        pc.movementSpeed = pcBaseSpeed;
        joustingCollider.enabled = false;
        JOUSTING = false;
    }

    private IEnumerator JOUST() 
    {
        JOUSTING = true;
        joustingCollider.enabled = true;
        yield return new WaitForSeconds(joustTime);
        pc.movementSpeed = pcBaseSpeed;
        joustingCollider.enabled = false;
        JOUSTING = false;
    }

    void Update() 
    {
        if (JOUSTING) 
        {
            pc.movementSpeed += pcBaseSpeed * joustSpeedMultiplier * Time.deltaTime * 0.1f;
        }
    }
}
