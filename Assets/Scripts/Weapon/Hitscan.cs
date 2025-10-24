// Created By: Ryan Lupoli
// This script uses raycasts to deal damage to targets as soon as the fireaction is activated. In other words, it allows guns to utilize hitscan
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Hitscan : MonoBehaviour, IWeaponLevel
{
    #region Variable

    [Header("Damage Settings")]
    [Tooltip("The teamID of the Projectile")]
    [SerializeField] private float teamID;
    [Tooltip("The amount of damage that the weapon begins with")]
    [SerializeField] private float baseDamage;
    [Tooltip("How much damage is added per level")]
    [SerializeField] private float levelDamage;
    private float cummulativeDamage;
    private WeaponLevel currentWeaponLevel;


    [Header("Spread & Range Settings")]
    [Tooltip("Determines where the raycast will be shot from")]
    [SerializeField] private Transform bulletSpawnPoint;
    [Tooltip("Determines if the weapon has random \"Bullet\" Spread")]
    [SerializeField] private bool addBulletSpread = true;
    [Tooltip("Determines how much random \"Bullet\" Spread the weapon has")]
    [SerializeField] private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [Tooltip("Determines how range (in Units) the weapon has")]
    [SerializeField] private float maxRange = 1000f;

    [Header("Ricochet Settings")]
    [Tooltip("Determines whether or not the weapon can ricochet.")]
    [SerializeField] private bool canRicochet = false;
    [Tooltip("Determines how many times a bullet is allowed to ricochet before despawning. Must be a value greater than 0 for Ricochet to function.")]
    [SerializeField] private int maxBounces = 0;
    private int currentBounces;


    [Header("Visual Settings")]
    [Tooltip("The particle system that is used when the player fires the gun")]
    [SerializeField] private ParticleSystem shootingSystem;
    [Tooltip("The particle system used when a \"Bullet\" impacts something")]
    [SerializeField] private ParticleSystem impactSystem;
    [Tooltip("The trail left by \"Bullets\"")]
    [SerializeField] private TrailRenderer bulletTrail;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletSpawnPoint = GameObject.FindAnyObjectByType<Camera>().transform;
        currentWeaponLevel = GetComponent<WeaponLevel>();
        UpdateLevelDamage(); //set the initial damage at the level the weapon spawns at.
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot()
    {
        // Set the direction of the raycast to the bulletSpawnPoint
        Vector3 direction = bulletSpawnPoint.forward;

        // Potentially add random bullet spread
        if (addBulletSpread)
        {
            direction += new Vector3(Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x), Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y), Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
            direction.Normalize();
        }

        Vector3 origin = bulletSpawnPoint.position;
        float remainingDistance = maxRange;
        currentBounces = 0;
        
        // While the bullet is allowed to continue ricocheting
        while (remainingDistance > 0)
        {
            Ray ray = new Ray(bulletSpawnPoint.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, remainingDistance))
            {
                float distanceTraveled = Vector3.Distance(origin, hit.point);
                remainingDistance -= distanceTraveled;

                Vector3 hitPoint = hit.point;

                // If an impact system is assigned, play it where the bullet hit
                if (impactSystem != null)
                {
                    ParticleSystem impact = Instantiate(impactSystem, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, 1f);
                }

                // If a bulletTrail has been assinged, play it
                if (bulletTrail != null)
                {
                    TrailRenderer trail = Instantiate(bulletTrail, origin, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hitPoint));
                }

                // Deal damage to target object // MG - Grab the parent of the object with the Head and Body colliders 
                Health targetHealth = hit.collider.GetComponentInParent<Health>();
                // If the target object has a health component and the teamID is different than the one assigned to the weapon
                if (targetHealth != null && targetHealth.teamID != this.teamID)
                {
                    float appliedDamage = cummulativeDamage;

                    // MG - Compares collider with Tag Head to deal double dmg
                    if (hit.collider.CompareTag("Head"))
                    {
                        appliedDamage *= 2f;
                        Debug.Log("Hitscan: Headshot");
                    }
                    // MG - Compares collider with Tag Body to deal normal dmg
                    if (hit.collider.CompareTag("Body"))
                    {
                        appliedDamage = cummulativeDamage;
                        Debug.Log("Hitscan: Body shot.");
                    }
                    // Deal the weapon's damage to the target
                    targetHealth.TakeDamage(appliedDamage);

                    // Stop ricocheting after hitting a valid target
                    break;
                }
                // If the hitscan can ricochet, and the maximum amount of bounces has not been reached
                if (canRicochet && currentBounces < maxBounces)
                {
                    currentBounces++;
                    // Reflect the direction of the bullet
                    direction = Vector3.Reflect(direction, hit.normal);
                    // Create a new origin at an offset to prevent an immediate rehit
                    origin = hit.point + hit.normal * 0.01f;
                    
                    continue;
                }
                // Else, if there is no ricochet allowed, or the maximum amount of bounces has been reached
                else
                {
                    // Break out of the loop
                    break;
                }
            }
            // If nothing was hit, simulate the bullet trail anyways
            else
            {
                Vector3 hitPoint = origin + direction * remainingDistance;

                if (bulletTrail != null)
                {
                    TrailRenderer trail = Instantiate(bulletTrail, origin, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hitPoint));
                }

                break;
            }
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= Time.deltaTime * 200f; // Bullet speed
            yield return null;
        }

        trail.transform.position = hitPoint;

        Destroy(trail.gameObject, trail.time);
    }

    //Calculates the cummulativeDamage that the weapon should do, this function should be called on the weapon levelling up.
    //Simple floor + (lvl * weaponLvlDamage)
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage + (levelDamage * (currentWeaponLevel.Level-1));
    }
    
    //Attempts to level the weapon up, if the weapon levels up the damage needs to be recalculated.
    //This function should be called by the shops or anything that upgrades the weapons level.
    //Similarly, this function also returns the true or false if it was able to upgrade.
    public bool UpgradeWeapon()
    {
        bool ret = currentWeaponLevel.LevelUpWeapon();
        if (ret) UpdateLevelDamage();
        return ret;
    }

}
