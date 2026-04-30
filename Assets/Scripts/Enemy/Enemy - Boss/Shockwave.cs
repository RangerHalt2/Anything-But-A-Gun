using UnityEngine;
using System.Collections;

public class Shockwave : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float teamID = 1;

    [Header("Timing Settings")]
    [SerializeField] private float colliderActiveTime = 0.2f;
    [SerializeField] private float lifetime = 1f;

    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();

        StartCoroutine(DisableColliderAfterDelay(colliderActiveTime));
        Destroy(gameObject, lifetime);
    }

    private IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        col.enabled = false;
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