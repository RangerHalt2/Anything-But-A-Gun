// Created By: Ryan Lupoli
// This is a script designed to manage the behaviors of projectiles fired by weapons

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
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
    [SerializeField] public float damage;

    [Header("Special Properties")]
    [Tooltip("Determines whether the projectile is able to pierce through, and deal damage, to multiple objects.")]
    [SerializeField] private bool piercing;
    [Tooltip("Determines whether the projectile will impact with or ricochet off of walls.")]
    public WallBehavior wallBehavior;
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
        bounceCooldown -= Time.deltaTime;
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

    // Collision Detection
    void OnTriggerEnter(Collider collider)
    {
        // Attempt to reference the health script on the collided object
        Health health = collider.gameObject.GetComponentInParent<Health>();

        // If the object has a health script
        if (health != null)
        {
            // If the teamID of the object is different than the teamID of the projectile
            if (health.teamID != teamID)
            {
                // Deal damage to targets health equal to projectile's damage
                health.TakeDamage(damage);
                // If the projectile cannot pierce...
                if (!piercing)
                {
                    // Destroy the projectile
                    Destroy(gameObject);
                }
            }
        }

        // If the projectile collides with a wall...
        if (collider.gameObject.CompareTag("Wall"))
        {
            switch (wallBehavior)
            {
                // Impact
                case WallBehavior.Impact:
                    // Destroy Projectile
                    Destroy(gameObject);
                    break;
                // Ricochet
                case WallBehavior.Ricochet:
                    SimulateRicochet(collider);
                    break;
                // Default Case, should never be called
                default:
                    break;
            }
        }
    }

    #region Special Properties
    private void SimulateRicochet(Collider collider)
    {
        if (bounceTimer > 0) return;
        // If the current bounces are greater than or equal to the maximum amount of bounces...
        if (currentBounces >= maxBounces)
        {
            // Destroy the projectile
            Destroy(gameObject);
            return;
        }
        // Do a raycast from the current postion backwards to try and get the normal
        RaycastHit hit;
        Vector3 direction = rb.linearVelocity.normalized;

        if(Physics.Raycast(transform.position, direction, out hit, 1.0f))
        {
            Vector3 normal = hit.normal;
            Vector3 reflectedDirection = Vector3.Reflect(direction, normal);

            // Update Projectile velocity
            rb.linearVelocity = reflectedDirection * speed;
            // Rotate projectile to face the new direction
            transform.forward = reflectedDirection;
            // Increment currentBounces
            currentBounces++;
            bounceTimer = bounceCooldown;
        }
        else
        {
            Debug.Log("Richochet did NOT hit a wall with it's raycast!");
        }
    }
    #endregion
}
