using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class BlackholeScript : MonoBehaviour, IWeaponLevel
{

    public float targetScale = 5.0f; // Max projectile growth
    public float growthSpeed = 0.65f; // How fast the projectile grows per second

    private Vector3 initialScale;
    private bool growing = true;

    public float timer;
    [SerializeField] private float baseDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] private float growthRate;
    [SerializeField] private float blackholeRadius;
    [SerializeField] private float blackholeForce;
    private float cummulativeDamage;

    private WeaponLevel currentWeaponLevel;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
        if (currentWeaponLevel != null) UpdateLevelDamage();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;

        if (timer < 0) 
        {
            Destroy(gameObject);
        }

        if (growing)
        {
            Vector3 newScale = transform.localScale;

            // Increase scale gradually
            newScale.x += growthSpeed * Time.deltaTime;
            newScale.y += growthSpeed * Time.deltaTime;
            newScale.z += growthSpeed * Time.deltaTime;

            transform.localScale = newScale;

            baseDamage = (maxDamage / 1) * (1 / (transform.localScale.x / initialScale.x));

            // Stop growing when target scale is reached
            if (transform.localScale.x >= initialScale.x * targetScale)
            {
                growing = false;
            }

            blackholeRadius = newScale.x;
        }

    }

    void OnTriggerEnter(Collider _other) 
    {
        Rigidbody rb = _other.gameObject.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
    void OnTriggerExit(Collider _other) 
    {
        Rigidbody rb = _other.gameObject.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnTriggerStay(Collider _other)
    {
        // Attempt to reference the health script on the collided object
        Health health = _other.gameObject.GetComponentInParent<Health>();
        if(health == null)
        {
            health = _other.gameObject.GetComponentInChildren<Health>();
        }

        // If the object has a health script
        if (health != null)
        {
            // Deal damage to targets health equal to projectile's damage

            if (_other.GetComponentInParent<NavMeshAgent>() != null)
            {
                //_other.GetComponentInParent<NavMeshAgent>().enabled = false;
                Vector3 direction = (_other.gameObject.transform.position - (Vector3)transform.position).normalized;
                float distance = Vector3.Distance(transform.position, _other.gameObject.transform.position);
                float forceMultiplier = 1f - (distance / blackholeRadius); // Force decreases with distance
                health.TakeDamage(cummulativeDamage * (distance / blackholeRadius), this.transform);

                Rigidbody rb = _other.gameObject.GetComponentInParent<Rigidbody>();
                rb.AddForce(-direction * blackholeForce * (distance / blackholeRadius), ForceMode.Impulse);
            }
        }
    }

    void OnDestroy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blackholeRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.GetComponentInParent<Rigidbody>() != null)
            {
                Rigidbody rb = hitCollider.GetComponentInParent<Rigidbody>();
                rb.isKinematic = true;
            }
        }
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
    }
}
