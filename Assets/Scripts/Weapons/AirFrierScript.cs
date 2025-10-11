using UnityEngine;

public class AirFrierScript : MonoBehaviour, IWeapon
{
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    private float lastFired = Mathf.NegativeInfinity;

    [SerializeField] private float timeOnBox = 5f; //Time to cook the nuggets
    [SerializeField] private float cookTime = 0f; //Current time the nugget has been cooking
    [SerializeField] private float cookSpeed = 1f; //Temperature of the oven, ha ha
    [SerializeField] private float coolSpeed = 0.1f; //Speed that the nuggies cool off

    //NEEDS UI FOR CHARGE TIME

    private void Awake()
    {
        cookTime = 0f;
    }

    private void Update() 
    { 
        cookTime -= coolSpeed;
    }
    
    public void Shoot()
    {
            // If enough time has passed since the last round was fired
        if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
        {

            if (cookTime > timeOnBox)
            {
                // If there is an assigned ammo manager, and that ammo manager has at least one round of ammo loaded
                if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
                {
                    // Attempt to fire the weapon
                    ammoManager.Fire();
                    // If the weapon is not reloading
                    if (!ammoManager.IsReloading())
                    {

                        if (projectilePrefab != null)
                        {
                            SpawnProjectile();
                            cookTime = 0f;
                        }
                        // Update lastFired
                        lastFired = Time.timeSinceLevelLoad;

                    }
                }
            }
            else
            {
                cookTime += cookSpeed;
            }
        }
        
    }

    public void Reload()
    {
        // If the shooter has at least one round of reserve ammo or is set to have infinite ammo
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            // Reload the shooter
            ammoManager.ReloadWeapon();
            cookTime = 0f;
        }
    }

    void SpawnProjectile()
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
        }
    }
}
