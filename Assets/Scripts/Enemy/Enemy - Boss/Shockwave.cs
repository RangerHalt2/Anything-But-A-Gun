using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float teamID = 1;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            if (health.teamID == teamID) return;

            health.TakeDamage(damage, this.transform);
        }
    }
}