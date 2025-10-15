using UnityEngine;

public class LavaPuddleScript : MonoBehaviour
{
    [SerializeField] private float puddleTimer;
    [SerializeField] private float damage;

    // Update is called once per frame
    void Update()
    {
        puddleTimer -= 1 * Time.deltaTime;

        if (puddleTimer < 0) 
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        // Attempt to reference the health script on the collided object
        Health health = _other.gameObject.GetComponentInParent<Health>();

        // If the object has a health script
        if (health != null)
        {
            // Deal damage to targets health equal to projectile's damage
            health.TakeDamage(damage);
        }
    }
}
