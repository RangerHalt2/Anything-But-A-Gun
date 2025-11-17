using UnityEngine;

public class TollBoxScript : WeaponClass
{
    [SerializeField] private Transform projectileSpawnPoint;
    private WeaponLevel weaponLevelRef;
    [SerializeField] private float possessionTimer;

    public override void SpawnProjectile()
    {
        // Check that the prefab is valid
        if (projectilePrefab != null)
        {
            // Create the projectile
            GameObject projectileGameObject = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, transform.rotation, null);

            // Account for spread
            Vector3 rotationEulerAngles = projectileGameObject.transform.rotation.eulerAngles;
            projectileGameObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);

            // Keep the heirarchy organized
            if (projectileSpawnPoint == null && GameObject.Find("ProjectileSpawnPoint") != null)
            {
                projectileSpawnPoint = GameObject.Find("ProjectileSpawnPoint").transform;

            }
            Projectile proj = projectileGameObject.GetComponent<Projectile>();
            proj.SetWeaponLevelReference(weaponLevelRef);

            TollBoxProjectileScript tollProj = projectileGameObject.GetComponent<TollBoxProjectileScript>();
            tollProj.SetWeaponLevelReference(weaponLevelRef);
            tollProj.timer = possessionTimer;
        }
    }
}
