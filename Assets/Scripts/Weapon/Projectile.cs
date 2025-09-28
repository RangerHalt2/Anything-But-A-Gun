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
            Debug.LogWarning(gameObject.name + " is missing a rigidbody. Projectile Script cannot function properly without it!");
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        Health health = collider.gameObject.GetComponent<Health>();

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
                    // Currently Unimplemented
                    break;
                // Default Case, should never be called
                default:
                    break;
            }
        }
    }
}
