using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DuckGooseProjectile : MonoBehaviour
{
    [Header("Duck Duck Goose Projectile Settings")]
    [SerializeField] private float gooseMulti = 3f;
    [SerializeField] private float range = 10f;
    [SerializeField] private int maxBounce = 5;
    [SerializeField] private float height = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float chanceToGoose = 15f;
    [SerializeField] private LayerMask whatIsEnemy;

    private List<Health> enemiesHit;

    private int currentBounces = 0;

    private Projectile projectileRef;
    private float teamID;
    private float baseDamage;
    private float growthRate;
    private float cummulativeDamage;
    private WeaponLevel currentWeaponLevel;

    private void Start()
    {
        enemiesHit = new List<Health>();
        projectileRef = GetComponent<Projectile>();
        teamID = projectileRef.GetTeamID();
        baseDamage = projectileRef.GetBaseDamage();
        growthRate = projectileRef.GetGrowthRate();
    }

    public void BounceToEnemy()
    {
        Transform closestEnemy = GetClosestEnemy();
        if (closestEnemy == null)
        {
            Debug.Log("Could not find an enemy nearby");
            Destroy(gameObject); // no valid target
            return;
        }
        StartCoroutine(ArcToTarget(closestEnemy, duration, height));
    }

    private IEnumerator ArcToTarget(Transform target, float duration, float height)
    {
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = Vector3.zero;
        Vector3 start = transform.position;
        float elapsed = 0f;
        //transform.LookAt(end);
        while (elapsed < duration)
        {
            if (target == null) yield break;
            float t = elapsed / duration; //This is the current "point" of the arc
            Vector3 end = target.position + Vector3.up * 0.3f; // adjust to enemy head height
            // Horizontal linear interpolation
            Vector3 pos = Vector3.Lerp(start, end, t);
            // Add vertical parabolic offset: peak at t=0.5
            pos.y += height * 4f * t * (1 - t); // parabola formula
            transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to target position
        transform.position = target.position + Vector3.up * 0.3f;
    }

    private IEnumerator DelayedBounce()
    {
        yield return new WaitForEndOfFrame(); // small buffer
        BounceToEnemy();
    }

    private Transform GetClosestEnemy()
    {
        Transform closestEnemy = null;

        float closestDistance = Mathf.Infinity;

        Collider[] cols = Physics.OverlapSphere(transform.position, range, whatIsEnemy);
        foreach (Collider col in cols)
        {
            Health health = col.GetComponent<Health>();
            if (health == null) health = col.GetComponentInChildren<Health>();
            if (health == null) health = col.GetComponentInParent<Health>();
            if (health == null) continue;
            if (enemiesHit.Contains(health)) continue; //Skip enemies already damaged
            if (Vector3.Distance(transform.position, col.transform.position) < closestDistance)
            {
                closestEnemy = col.transform;
                closestDistance = Vector3.Distance(transform.position, col.transform.position);
            }
        }

        return closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health enemyHealth = other.GetComponent<Health>();
        if (enemyHealth == null) enemyHealth = other.GetComponentInChildren<Health>();
        if (enemyHealth == null) enemyHealth = other.GetComponentInParent<Health>();
        if (enemyHealth == null) //If nothing just stop
        {
            Debug.Log("Enemy Health is null");
            return;
        }
        if (projectileRef.enabled){
            Debug.Log("Turning off projectile enabled and doing first hit");
            projectileRef.enabled = false;
            enemiesHit.Add(enemyHealth);
            StartCoroutine(DelayedBounce());
            return; //Don't do damage if the projectile script is still in control
        }
        
        float random = Random.Range(0f, 100f);
        if(random <= chanceToGoose || currentBounces == maxBounce) //Goose Case, random chance or last bounce
        {
            Debug.Log("Goosing");
            UpdateLevelDamage(); //Check the damage final time
            cummulativeDamage = cummulativeDamage * gooseMulti; //Set the damage to the GOOSE damage.
            enemyHealth.TakeDamage(cummulativeDamage); //Special take damage to cut earlier than usual with goose multi set
            Destroy(gameObject);
            return;
        }
        if (enemyHealth != null) {
            Debug.Log("Triggered the main bounce implementation code.");
            projectileRef.enabled = false;
            DoDamage(enemyHealth);
            enemiesHit.Add(enemyHealth);
            currentBounces++;
        }
        if(currentBounces <= maxBounce)
        {
            Debug.Log("Starting a new bounce!");
            StartCoroutine(DelayedBounce());
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
        }
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
    }

    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level - 1);
    }
}
