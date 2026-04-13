using UnityEngine;

public class PidgeonAreaOfEffect : MonoBehaviour, IWeaponLevel
{


    [SerializeField] private float baseDamage;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private float radius;
    [SerializeField] private float timer;
    [SerializeField] private GameObject[] pidgeonSFX; // Added by Aaron 4/12/26
    private float cummulativeDamage;

    public bool followingTarget = false;

    private WeaponLevel currentWeaponLevel;

    void Awake()
    {
        Invoke("Explode", timer);
        RandomGunShot(transform);
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
        if (currentWeaponLevel != null) UpdateLevelDamage();
    }

    private void Explode()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);
        
        foreach (Collider other in enemies)
        {
            if (other.CompareTag("WeakPoint")) continue;
            Health health = other.gameObject.GetComponentInParent<Health>();
            if(health == null)
                health = other.gameObject.GetComponentInChildren<Health>();
            // If the object has a health script
            if (health != null && health.gameObject.tag != "Player")
            {
                // Deal damage to targets health equal to projectile's damage
                Debug.Log("PIDGEON AREA OF EFFECT - Dealing Damage to this chud");
                health.TakeDamage(baseDamage, this.transform);
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (followingTarget) return; //If following target the rest is unnecessary
        Health enemyHealth = other.transform.root.GetComponentInParent<Health>();
        if (enemyHealth == null)
        {
            enemyHealth = other.transform.root.GetComponentInChildren<Health>();
        }
        if (enemyHealth == null || enemyHealth.teamID == 0)
        {
            Debug.Log("PIDGEON AREA OF EFFECT - The enemy was either not found or is a player, returning.");
            return; //If no enemy, or the health found is the player's
        }

        Debug.Log("PIDGEON AREA OF EFFECT - The enemy has been assigned their own pidgeon bomb");
        this.gameObject.transform.SetParent(other.gameObject.transform, true);
        followingTarget = true;
    }

    public void RandomGunShot(Transform followTrans) // Added by Aaron 4/12/26
    {
        int num = UnityEngine.Random.Range(0, pidgeonSFX.Length);
        GameObject selected = pidgeonSFX[num];
        GameObject randomShot = Instantiate(selected, followTrans.position, Quaternion.identity);
        //MovingAudio movingAudio = randomShot AddComponent<MovingAudio>();
    }

    //LB: Updates the weapon's damage for what damage it should do.
    //LB: This function is now Deprecated and should no longer be used while it's being removed
    public void UpdateLevelDamage()
    {
        //cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
    }
}
