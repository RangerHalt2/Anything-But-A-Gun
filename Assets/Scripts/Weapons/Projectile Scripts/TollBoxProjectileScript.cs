using UnityEngine;

public class TollBoxProjectileScript : MonoBehaviour
{
    public float timer = 1f;
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
        targetHealth = _other.gameObject.GetComponentInParent<Health>();
        if (targetHealth != null)
        {
            if (proj.baseDamage >= targetHealth.currentHealth && targetHealth.gameObject.AddComponent<PossessedEffect>() == null)
            {
                PossessedEffect pe = targetHealth.gameObject.AddComponent<PossessedEffect>();
                pe.timer = timer;
            }
        }
    }
}
