using UnityEngine;

public class LavaPuddleScript : MonoBehaviour, IWeaponLevel
{
    public float puddleTimer;
    [SerializeField] private float baseDamage;
    [SerializeField] private float growthRate = 1.15f;
    [HideInInspector] public float externalDamageMod = 1f;
    private float cummulativeDamage;

    private WeaponLevel currentWeaponLevel;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
        if (currentWeaponLevel != null) UpdateLevelDamage();
    }

    // Update is called once per frame
    void Update()
    {
        puddleTimer -= Time.deltaTime;

        if (puddleTimer < 0) 
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Health health = collision.gameObject.GetComponentInParent<Health>();
        OnColliderStay(health);
    }

    private void OnTriggerStay(Collider other)
    {
        Health health = other.gameObject.GetComponentInParent<Health>();
        OnColliderStay(health);
    }

    //LB: On Collider Stay isn't a real function??? I moved this call to OnCollisionStay.
    void OnColliderStay(Health health)
    {
        if (health != null && health.teamID != 0)
        {
            // Deal damage to targets health equal to projectile's damage
            health.TakeDamage(cummulativeDamage * Time.deltaTime, this.transform);
        }
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
        cummulativeDamage *= externalDamageMod;
    }
}
