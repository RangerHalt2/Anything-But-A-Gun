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
    [SerializeField] private float speed;

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
    private float cummulativeDamage;
    private WeaponLevel currentWeaponLevel;
        
    [Header("Special Properties")]
    [Tooltip("Determines whether the projectile is able to pierce through, and deal damage, to multiple objects.")]
    [SerializeField] private bool piercing;
    [Tooltip("Determines whether the projectile will impact with or ricochet off of walls.")]
    public WallBehavior wallBehavior;
    [Tooltip("Determines if it's AOE or not")]
    [SerializeField] private bool IsAoe;
    [Tooltip("AOE Range that it can hit people in")]
    [SerializeField] private float aoeRange;
    [Tooltip("Determines the maximum amount of bounces. Only utilized if the projectile has the Ricochet wall behavior. Must be a value greater than 0.")]
    [SerializeField] private int maxBounces = 0;
    // How many times the projectile has bounced
    private int currentBounces = 0;

    //LB: Adding a cooldown on how often the ball can bounce, just by a fraction of a second
    private float bounceCooldown = 0.2f;
    private float bounceTimer = 0;
    
    [Tooltip("Determines whether the projectile will be effected by gravity.")]
    [SerializeField] private bool bulletDrop;
    [Tooltip("Multiplier for gravity when bullet drop is enabled.")]
    [SerializeField] private float gravityMultiplier = 1.0f;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
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
            // Add speed to the velocity of the projectile
            rb.linearVelocity = transform.forward * speed;

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
    }

    void FixedUpdate()
    {
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
        if (IsAoe) {
            ApplyAOE(this.transform);
            return;
        }
        // Access hit object's health script
        Health health = collider.gameObject.GetComponentInParent<Health>();

        // If health script is found...
        if (health != null)
        {
            DoDamage(health);
        }

        // Only destroy on impact if wall behavior is Impact 
        if (collider.gameObject.CompareTag("Wall") && wallBehavior == WallBehavior.Impact)
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
                cummulativeDamage = baseDamage;
            // Deal damage
            health.TakeDamage(cummulativeDamage);
            // If projectile is not piercing...
            if (!piercing)
            {
                // Destroy the projectile
                Destroy(gameObject);
            }
        }
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level-1);
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
            Health enemyHealth = col.GetComponent<Health>();
            if (enemyHealth == null) continue;
            DoDamage(enemyHealth);
        }
    }
    #endregion
}
