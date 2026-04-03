// Created By: Ryan Lupoli
// This is a script designed to manage the behaviors of projectiles fired by weapons

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour, IWeaponLevel
{
    #region Variables
    [Header("Speed Settings")]
    [Tooltip("The speed at which the projectile will travel.")]
    public float speed; //EW: I made this public instead of private so it can be modified/referenced outside of this script.

    [Header("Custom Mortar Settings")] //MG: Added Mortar Settings
    [SerializeField] private bool useCustomMortar = false;
    [SerializeField] private float customGravity = 20f;
    [SerializeField] private float launchAngle = 45f;
    private Vector3 currentVelocity;


    [Header("Lifetime Settings")]
    [Tooltip("The amount of time (in seconds) a projectile will exist for.")]
    [SerializeField] private float lifeTime;

    [Header("Damage Settings")]
    [Tooltip("The teamID of the Projectile")]
    [SerializeField] private float teamID;
    [Tooltip("The amount of damage dealt to a target")]
    [SerializeField] public float baseDamage;
    [Tooltip("Damage to be added per level to the projectile")]
    [SerializeField] private float growthRate = 1.15f;

    [Space]
    [Tooltip("Determines whether or not the projectile can deal bonus damage for hitting an enemy's weakpoint.")]
    [SerializeField] private bool canCrit;
    [Tooltip("A multiplier added to a weapon's damage when hitting a weakpoint.")]
    [SerializeField] private float critMult = 2f;
    // Tracks whether or not the projectile was a critical hit
    private bool criticalHit;
    [Space]
    private float cummulativeDamage;
    [HideInInspector] public float externalDmgMod = 1f;
    private WeaponLevel currentWeaponLevel;
    //EW: Nonlethal add
    [Tooltip("Determines whether the projectile can kill the enemy or not.")]
    [SerializeField] private bool nonlethal;

    [Header("Special Properties")]
    [Tooltip("Determines whether the projectile is able to pierce through, and deal damage, to multiple objects.")]
    [SerializeField] private bool piercing;
    [Space]
    [Tooltip("a unique weapon that should not destroy the projectile on impact")]
    [SerializeField] private bool unique;
    [Tooltip("Determines whether the projectile will impact with or ricochet off of walls.")]
    public WallBehavior wallBehavior;
    [Space]
    [Tooltip("Determines the maximum amount of bounces. Only utilized if the projectile has the Ricochet wall behavior. Must be a value greater than 0.")]
    [SerializeField] private int maxBounces = 0;
    // How many times the projectile has bounced
    private int currentBounces = 0;
    //LB: Adding a cooldown on how often the ball can bounce, just by a fraction of a second
    private float bounceCooldown = 0.2f;
    private float bounceTimer = 0;
    [Space]
    [Tooltip("Determines whether the projectile will be effected by gravity.")]
    [SerializeField] private bool bulletDrop;
    [Tooltip("Multiplier for gravity when bullet drop is enabled.")]
    [SerializeField] private float gravityMultiplier = 1.0f;
    [Space]
    [Tooltip("Determines if it's AOE or not")]
    [SerializeField] private bool IsAoe;
    [Tooltip("AOE Range that it can hit people in")]
    [SerializeField] public float aoeRange;
    [SerializeField] private AudioClip[] ricochetSFX;
    private float ricochetTimer;
    private float ricochetCooldown = 0.2f;
    [Space]
    [SerializeField] public GameObject spikedExplosionVFX;

    [HideInInspector] public Transform enemyPosition;

    public void SetIsAoE(bool isAoe)
    {
        this.IsAoe = isAoe;
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
    }

    public float GetTeamID()
    {
        return teamID;
    }
    
    public float GetBaseDamage()
    {
        return baseDamage;
    }

    public float GetGrowthRate()
    {
        return growthRate;
    }

    public WeaponLevel GetCurrentWeaponLevel()
    {
        return currentWeaponLevel;
    }

    public void SetGravityScale(float gravity)
    {
        this.gravityMultiplier = gravity;
    }

    // Referenece to the Projectile's's Rigidbody
    private Rigidbody rb;
    // Enum of potential Wall Interactions
    public enum WallBehavior
    {
        Impact,
        Ricochet
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Turn off default gravity since we'll be using a custom one
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Check if Rigidbody is present
        if (rb != null)
        {
            //MG: Added Calculation for Mortars
            // Add speed to the velocity of the projectile
            if (useCustomMortar && enemyPosition != null)
            {
                currentVelocity = CalculateLaunchVelocity(enemyPosition.position);
            }
            else
            {
                rb.linearVelocity = transform.forward * speed;
            }

            // Start Coroutine to despawn projectile after set delay
            StartCoroutine(DestroyAfterDelay());
        }
        else
        {
            Debug.LogWarning("Projectile: " + gameObject.name + " is missing a rigidbody. Projectile Script cannot function properly without it!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bounceTimer > 0)
            bounceTimer -= Time.deltaTime;
        ricochetTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    //MG: Added Mortar calculat
    {
        if (useCustomMortar)
        {
            // Apply custom gravity
            currentVelocity.y -= customGravity * Time.fixedDeltaTime;

            rb.linearVelocity = currentVelocity;

            // Rotate to face movement direction
            if (currentVelocity != Vector3.zero)
            {
                transform.forward = currentVelocity.normalized;
            }

            return;
        }

        if (!bulletDrop)
        {
            return;
        }
        else
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }
    }

    // Destroys the gameObject after a set delay defined by lifeTime
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("PROJECTILE - Destroy after Delay");
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        HandleCollision(collision.collider, contact.point, contact.normal);

        // Handle ricochet only on collision with walls and if ricochet is enabled
        if (collision.gameObject.CompareTag("Wall") && wallBehavior == WallBehavior.Ricochet)
        {
            SimulateRicochet(collision);
        }
    }

    // Collision Detection
    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other, other.ClosestPoint(transform.position), Vector3.zero);
    }

    private void HandleCollision(Collider collider, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsAoe && !collider.isTrigger) {
            ApplyAOE(this.transform);
            if (spikedExplosionVFX != null && ricochetTimer < 0)
            {
                Instantiate(spikedExplosionVFX, transform.position, transform.rotation, null);
                ricochetTimer = ricochetCooldown;
            }
                
            return;
        }
        // Access hit object's health script
        Health health = collider.gameObject.GetComponentInParent<Health>();

        // If health script is found...
        if (health != null)
        {
            if(collider.gameObject.CompareTag("WeakPoint"))
            {
                criticalHit = true;
            }
            DoDamage(health);
        }

        // Only destroy on impact if wall behavior is Impact EW: Added !unique to make unique weapons not destroy when in contact with the wall.
        if (collider.gameObject.CompareTag("Wall") && wallBehavior == WallBehavior.Impact && !unique)
        {
            Destroy(gameObject);
        }
    }

    private void DoDamage(Health health)
    {
        // If team ID is different...
        if (health.teamID != teamID)
        {
            if (currentWeaponLevel != null)
            {
                UpdateLevelDamage();
            }
            else
            {
                cummulativeDamage = baseDamage;
            }
            // If the projectile hit a weak point...
            if (canCrit && criticalHit)
            {
                // Multiply cumulative damage by the critMult
                cummulativeDamage *= critMult;
            }
            // EW: Deal nonlethal damage
            if (nonlethal)
            {
                health.TakeNonLethalDamage(cummulativeDamage);
            }
            else 
            {
                //Take damage
                health.TakeDamage(cummulativeDamage, enemyPosition);
            }
            // If projectile is not piercing...
            if (!piercing && !unique)
            {
                // Destroy the projectile
                //Debug.Log("Found the culprit");
                Destroy(gameObject);
            }
        }
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level-1);
        cummulativeDamage *= externalDmgMod;
    }

    #region Special Properties
   private void SimulateRicochet(Collision collision)
    {
        if (bounceTimer > 0) return;

        // If the projectile has reached its max bounces...
        if (currentBounces >= maxBounces)
        {
            // Destroy it
            Destroy(gameObject);
            return;
        }

        Vector3 normal = collision.contacts[0].normal;
        Vector3 incomingVelocity = rb.linearVelocity;

        // Reflect velocity using collision normal
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);

        // Lose energy on bounce
        // Keep this value at 1 to have no energy lost
        float energyLossFactor = 1f;
        reflectedVelocity *= energyLossFactor;

        // Ensure minimum velocity to avoid stopping completely
        float minVelocityMagnitude = 1f;
        if (reflectedVelocity.magnitude < minVelocityMagnitude)
        {
            reflectedVelocity = reflectedVelocity.normalized * minVelocityMagnitude;
        }

        // Set the velocity to the reflected velocity
        rb.linearVelocity = reflectedVelocity;

        if (reflectedVelocity != Vector3.zero)
        {
            transform.forward = reflectedVelocity.normalized;
        }

        if(ricochetSFX.Length > 0)
        {
            Debug.Log("attempt to play SFX");
            SoundEffectsManager.instance.PlayRandomSoundEffectClip(ricochetSFX, transform, 1f);
        }

        // Increment current bounces
        currentBounces++;
        // Reset the bounce timer
        bounceTimer = bounceCooldown;
    }

    private void ApplyAOE(Transform center)
    {
        Collider[] cols = Physics.OverlapSphere(center.position, aoeRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("WeakPoint")) continue;
            Health enemyHealth = col.GetComponentInParent<Health>();
            if(enemyHealth == null) enemyHealth = col.GetComponent<Health>();
            if(enemyHealth == null) enemyHealth = col.GetComponentInChildren<Health>();
            if (enemyHealth == null) continue;
            DoDamage(enemyHealth);
        }
    }

    private Vector3 CalculateLaunchVelocity(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;

            float yOffset = direction.y;
            direction.y = 0;

            float distance = direction.magnitude;

            float angleRad = launchAngle * Mathf.Deg2Rad;

            float g = customGravity;

            float velocity = Mathf.Sqrt(distance * g / Mathf.Sin(2 * angleRad));

            float velocityY = velocity * Mathf.Sin(angleRad);
            float velocityXZ = velocity * Mathf.Cos(angleRad);

            Vector3 result = direction.normalized * velocityXZ + Vector3.up * velocityY;

            return result;
        }
    #endregion
}
