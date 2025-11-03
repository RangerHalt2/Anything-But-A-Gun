using UnityEngine;

public class DuckDuckGoose : MonoBehaviour, IWeapon
{

    [SerializeField] public int level { get; set; }

    [SerializeField] private float fireRate = 0.75f;
    private AmmoManager ammoManager;
    [Tooltip("This will be the projectile that is fired when on the ground, the not 'spiked' prefab")]
    [SerializeField] private GameObject ProjectilePrefab;
    [Tooltip("Where the projectile should spawn from")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject gunShot;

    private WeaponLevel weaponLevelRef;

    private float timer = 0;

    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
        weaponLevelRef = GetComponent<WeaponLevel>();
    }

    public void Shoot()
    {
        if (timer < 0)
        {
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0 && !ammoManager.IsReloading())
            {
                ammoManager.Fire();
                SpawnProjectile();
                timer = fireRate;
            }
        }
    }

    private void SpawnProjectile()
    {
        if (ProjectilePrefab == null)
        {
            Debug.LogError("The Bullet for the VolleyBall was null, a tech likely forgot to assemble the weapon properly!");
            return;
        }

        GameObject projectile = Instantiate(ProjectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation, null);
        if(gunShot != null)
            Instantiate(gunShot, transform.position, transform.rotation, null);

        Vector3 rotationEulerAngles = projectile.transform.rotation.eulerAngles;
        projectile.transform.rotation = Quaternion.Euler(rotationEulerAngles);

        Projectile proj = projectile.GetComponent<Projectile>();
        DuckGooseProjectile proj2 = projectile.GetComponent<DuckGooseProjectile>();
        proj.SetWeaponLevelReference(weaponLevelRef);
        proj2.SetWeaponLevelReference(weaponLevelRef);
    }

    public void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            ammoManager.ReloadWeapon();
        }
    }

    //Update Timers 
    private void Update()
    {
        timer -= Time.deltaTime;
    }
}
