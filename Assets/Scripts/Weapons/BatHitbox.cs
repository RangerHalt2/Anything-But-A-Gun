using UnityEngine;

public class BatHitbox : MonoBehaviour
{
    [SerializeField] int teamID = 0;
    [SerializeField] private float damage;

    private BaseballBat_V2 baseballBat;

    void Start()
    {
        baseballBat = GetComponentInParent<BaseballBat_V2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            // Don’t damage teammates
            if (health.teamID != teamID)
            {
                if (baseballBat != null)
                {
                    damage = baseballBat.GetCumulativeDamage();
                    baseballBat.RegisterHit();
                }
                // Deal damage
                health.TakeDamage(damage);
            }
        }
    }
}
