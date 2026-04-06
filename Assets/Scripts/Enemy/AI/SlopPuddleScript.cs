using UnityEngine;

public class SlopPuddleScript : MonoBehaviour
{
    public float puddleTimer;
    [SerializeField] private float baseDamage;
    [HideInInspector] public float externalDamageMod = 1f;

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
        Health health = _other.gameObject.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(baseDamage * externalDamageMod * Time.deltaTime, transform);
        }
    }
}