using System;
using UnityEngine;

public class PidgeonProjectile : MonoBehaviour
{
    [SerializeField] private GameObject birdPrefab;

    private WeaponLevel weaponLevelRef;

    [HideInInspector] public Type promotion;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Vector3 collisionPoint = gameObject.transform.position;

        Health enemyHealth = collision.gameObject.GetComponentInParent<Health>();
        if (enemyHealth == null)
        {
            enemyHealth = collision.gameObject.GetComponentInChildren<Health>();
        }
        GameObject pidgeonAOE;
        if (enemyHealth != null && enemyHealth.teamID != 0)
            pidgeonAOE = Instantiate(birdPrefab, collisionPoint, Quaternion.identity, collision.gameObject.transform); //If enemy spawn them in their transform
        else
            pidgeonAOE = Instantiate(birdPrefab, collisionPoint, Quaternion.identity); //If no enemy just place them
        PidgeonAreaOfEffect pidgeonAOEScript = pidgeonAOE.GetComponent<PidgeonAreaOfEffect>();

        if (enemyHealth != null)
            pidgeonAOEScript.followingTarget = true;

        pidgeonAOEScript.SetWeaponLevelReference(weaponLevelRef);
        Destroy(gameObject);
    }
}
