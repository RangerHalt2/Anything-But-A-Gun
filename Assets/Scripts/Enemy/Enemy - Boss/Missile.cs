using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    public float delayBeforeActivation = 1.5f;
    public int damage = 20;
    public int teamID = 1;
    
    public Material activeMaterial;

    private Collider missileCollider;
    private Renderer missileRenderer;
    private Material originalMaterial;

    void Start()
    {
        missileCollider = GetComponent<Collider>();
        missileRenderer = GetComponent<Renderer>();

        if (missileCollider != null)
            missileCollider.enabled = false;

        if (missileRenderer != null)
            originalMaterial = missileRenderer.material;

        StartCoroutine(ActivateColliderAfterDelay());

        Destroy(gameObject, 2f);
    }

    private IEnumerator ActivateColliderAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeActivation);

        if (missileCollider != null)
            missileCollider.enabled = true;

        if (missileRenderer != null && activeMaterial != null)
            missileRenderer.material = activeMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            if (health.teamID == teamID) return;

            health.TakeDamage(damage, this.transform);
            Destroy(gameObject);
        }
    }
}