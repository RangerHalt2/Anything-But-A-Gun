using UnityEngine;

public class TollBoxProjectileScript : MonoBehaviour
{
    private Health targetHealth;
    private Projectile proj;
    private WeaponLevel weaponLevelRef;

    void Start() 
    {
        proj = GetComponent<Projectile>();
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnTriggerEnter(Collider _other) 
    { 
        targetHealth = _other.gameObject.GetComponent<Health>();
        if (targetHealth != null)
        {
            if (proj.baseDamage > targetHealth.currentHealth)
            {
                targetHealth.ReceiveHealing(targetHealth.maxHealth - targetHealth.currentHealth);
                targetHealth.gameObject.AddComponent<PossessedEffect>();
            }
        }
    }
}
