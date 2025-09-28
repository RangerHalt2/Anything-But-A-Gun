// Created By: Ryan Lupoli
// This script uses raycasts to deal damage to targets as soon as the fireaction is activated. In other words, it allows guns to utilize hitscan
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    #region Variable

    [Header("Damage Settings")]
    [Tooltip("The teamID of the Projectile")]
    [SerializeField] private float teamID;
    [Tooltip("The amount of damage dealt to a target")]
    [SerializeField] private float damage;

    [Header("Spread & Range Settings")]
    [Tooltip("Determines where the raycast will be shot from")]
    [SerializeField] private Transform bulletSpawnPoint;
    [Tooltip("Determines if the weapon has random \"Bullet\" Spread")]
    [SerializeField] private bool addBulletSpread = true;
    [Tooltip("Determines how much random \"Bullet\" Spread the weapon has")]
    [SerializeField] private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [Tooltip("Determines how range (in Units) the weapon has")]
    [SerializeField] private float maxRange = 1000f;


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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Shoot()
    {
        Debug.Log("BANG!");

        // Set the direction of the raycast to the bulletSpawnPoint
        Vector3 direction = bulletSpawnPoint.forward;

        // Potentially add random bullet spread
        if (addBulletSpread)
        {
            direction += new Vector3(Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x), Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y), Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
        }

        Ray ray = new Ray(bulletSpawnPoint.position, direction);
        RaycastHit hit;

        Vector3 hitPoint;
        if (Physics.Raycast(ray, out hit, maxRange))
        {
            hitPoint = hit.point;

            // If an impact system is assigned, play it where the bullet hit
            if (impactSystem != null)
            {
                ParticleSystem impact = Instantiate(impactSystem, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1f);
            }

            // Deal damage to target object
            Health targetHealth = hit.collider.GetComponent<Health>();
            // If the target object has a health component and the teamID is different than the one assigned to the weapon
            if (targetHealth != null && targetHealth.teamID != this.teamID)
            {
                // Deal the weapon's damage to the target
                targetHealth.TakeDamage(damage);
            }
        }
        else
        {
            // If nothing was hit, simulate trail for point
            hitPoint = ray.origin + ray.direction * maxRange;
        }
        // Render bullet trail, if assigned
        if (bulletTrail != null)
        {
            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hitPoint));
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
}
