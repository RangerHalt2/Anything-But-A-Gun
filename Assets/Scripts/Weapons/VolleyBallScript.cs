using UnityEngine;

public class VolleyBallScript : MonoBehaviour, IWeapon
{
    
    [SerializeField] private float fireRate = 0.75f;
    private AmmoManager ammoManager;
    private CharacterController characterController;
    [Tooltip("This will be the projectile that is fired when on the ground, the not 'spiked' prefab")]
    [SerializeField] private GameObject groundProjectilePrefab;
    [Tooltip("This will be the projectile that is fired when in the air, the 'spiked' prefab")]
    [SerializeField] private GameObject airProjectilePrefab;
    [Tooltip("Where the projectile should spawn from")]
    [SerializeField] private Transform projectileSpawnPoint;

    private float timer = 0;

    private void Start()
    {
        ammoManager = GetComponent<AmmoManager>();
        characterController = GameObject.FindAnyObjectByType<CharacterController>(); //There should only be one Character Controller but if there are more than that this will need to change
    }

    //If the player is not reloading and this function is called it should check if the character controller is grounded and spawn the correct projectile
    public void Shoot()
    {
        if (timer < 0)
        {
            if(ammoManager!= null && ammoManager.GetCurrentAmmo() > 0 )
            {
                if (!ammoManager.IsReloading())
                {
                    ammoManager.Fire();
                    SpawnProjectile((characterController.isGrounded ? groundProjectilePrefab : airProjectilePrefab)); //Inline Bool check, ground if grounded and air if not grounded 
                    timer = fireRate;
                }
            }
        }
    }

    //Check the reserve ammo of the ammo manager and reload the weapon
    public void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1) 
        {
            ammoManager.ReloadWeapon();
        }
    }

    //This code handles the actual spawning of the projectile dependent on the bullet passed in the Shoot() function
    private void SpawnProjectile(GameObject bullet)
    {
        if(bullet == null)
        {
            Debug.LogError("The Bullet for the VolleyBall was null, a tech likely forgot to assemble the weapon properly!");
            return;
        }

        GameObject projectile = Instantiate(bullet, projectileSpawnPoint.position, projectileSpawnPoint.rotation, null);

        Vector3 rotationEulerAngles = projectile.transform.rotation.eulerAngles;
        projectile.transform.rotation = Quaternion.Euler(rotationEulerAngles);
    }

    //Update Timers 
    private void Update()
    {
        timer -= Time.deltaTime;
    }
}
