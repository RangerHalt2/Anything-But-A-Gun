using System.Collections;
using UnityEngine;

public class JoustingHorseScript : WeaponClass
{
    //[SerializeField] private GameObject joustingCollider;
    private Rigidbody joustingRB;
    public Collider joustingCollider;
    private JoustingColliderScript jCS;
    [SerializeField] private float joustForce; //How fast you joust
    [SerializeField] private float joustTime; //How long the joust is
    [SerializeField] private float joustCD;
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] private ParticleSystem rainbowSparkleLines;
    private float joustCharge; //Time before next joust 
    [SerializeField] private float timeBetweenJoustCharges;
    private int maxJousts;
    private int currentJousts;
    private float currentDamage;
    private PlayerController pc;
    private float pcBaseSpeed;

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
        if (rainbowSparkleLines != null)
        {
            rainbowSparkleLines.Stop();
        }
    }

    public override void Shoot()
    {
        // If enough time has passed since the last round was fired
        if (pc.canDash && timer <= 0)
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
    private void OnDisable()
    {
        StopCoroutine(JOUST());
        pc.movementSpeed = pcBaseSpeed;
        joustingCollider.enabled = false;
        pc.canDash = true;
        if(rainbowSparkleLines != null)
            rainbowSparkleLines.Stop();
    }

    private IEnumerator JOUST() 
    {
        pc.canDash = false;
        joustingCollider.enabled = true;

        if (rainbowSparkleLines != null)
            rainbowSparkleLines.Play();

        float startTime = Time.time;
        Vector3 dashDirection = Camera.main.gameObject.transform.forward;
        if (dashDirection.sqrMagnitude < 0.1 * 0.1f)
        {
            dashDirection = pc.gameObject.transform.forward;
        }

        //EW: Need a null check to make sure the dash doesn't fail
        if (gunShot != null)
        {
            // Play sound effect (added by Aaron)
            Instantiate(gunShot, transform.position, transform.rotation, null);
        }
        PlayOnomatopeia();
        while (Time.time < startTime + joustTime)
        {
            pc.GetPlayerCharacterController().Move(dashDirection * pc.movementSpeed * joustForce * Time.deltaTime);
            yield return null;
        }
        if (rainbowSparkleLines != null) rainbowSparkleLines.Stop();

        yield return new WaitForSeconds(joustCD); //Time before new joust can start
        joustingCollider.enabled = false;
        pc.canDash = true;
    }

    void Update() 
    {
        timer -= Time.deltaTime;
        maxJousts = ammoManager.GetAmmoCapacity();
        currentJousts = ammoManager.GetCurrentAmmo();

        if (currentJousts < maxJousts)
        {
            joustCharge += Time.deltaTime;
        }
        else 
        {
            joustCharge = 0;
        }

        if (joustCharge >= timeBetweenJoustCharges) 
        {
            ammoManager.ReloadWeapon();
            joustCharge = 0;
        }
    }

    public override void Reload() 
    { 
        //Reload doesn't work normally on this weapon. Like, on purpose. It reloads it's charges overtime
    }
}
