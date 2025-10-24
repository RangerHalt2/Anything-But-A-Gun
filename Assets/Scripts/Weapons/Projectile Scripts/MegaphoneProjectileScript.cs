using UnityEngine;

public class MegaphoneProjectileScript : MonoBehaviour
{
    public float targetScale = 5.0f; // Max projectile growth
    public float growthSpeed = 0.65f; // How fast the projectile grows per second

    private Vector3 initialScale;
    private bool growing = true;

    public Projectile projectile;

    public float maxDamage = 5.0f;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (growing)
        {
            Vector3 newScale = transform.localScale;

            // Increase scale gradually
            newScale.x += growthSpeed * Time.deltaTime;
            newScale.y += growthSpeed * Time.deltaTime;

            transform.localScale = newScale;

            projectile.baseDamage = (maxDamage / 1) * (1 / (transform.localScale.x / initialScale.x));

            // Stop growing when target scale is reached
            if (transform.localScale.x >= initialScale.x * targetScale)
            {
                growing = false;
            }
        }
    }
}
