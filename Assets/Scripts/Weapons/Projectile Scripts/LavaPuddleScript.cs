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

    void OnCollisionStay(Collision _other)
    {
        // Attempt to reference the health script on the collided object
        Health health = _other.gameObject.GetComponentInParent<Health>();

        // If the object has a health script
        if (health != null)
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
