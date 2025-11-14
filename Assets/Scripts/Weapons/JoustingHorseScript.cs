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
                    StartCoroutine(JOUST());
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
                }
            }
       
        }
    }

    private void OnDisable()
    {
        StopCoroutine(JOUST());
        joustingCollider.enabled = false;
        JOUSTING = false;
    }

    private IEnumerator JOUST() 
    {
        JOUSTING = true;
        joustingCollider.enabled = true;
        pc.movementSpeed = pcBaseSpeed * joustSpeedMultiplier;
        yield return new WaitForSeconds(joustTime);
        pc.movementSpeed = pcBaseSpeed;
        joustingCollider.enabled = false;
        JOUSTING = false;
    }
}
