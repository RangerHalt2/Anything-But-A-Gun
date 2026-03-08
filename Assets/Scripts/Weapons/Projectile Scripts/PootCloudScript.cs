using UnityEngine;

public class PootCloudScript : MonoBehaviour, IWeaponLevel
{
    public float targetScale = 5.0f; // Max projectile growth
    public float growthSpeed = 0.65f; // How fast the projectile grows per second

    private Vector3 initialScale;
    private bool growing = true;

    [SerializeField] private float cloudTimer;
    [SerializeField] private float baseDamage;
    [SerializeField] private float maxDamage;
    [SerializeField] private float growthRate;
    private float cummulativeDamage;

    private WeaponLevel currentWeaponLevel;

    void Awake()
    {
        initialScale = transform.localScale;
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
        if (currentWeaponLevel != null) UpdateLevelDamage();
    }

    // Update is called once per frame
    void Update()
    {
        cloudTimer -= 1 * Time.deltaTime;

        if (cloudTimer < 0) 
        {
            Destroy(gameObject);
        }

        if (growing)
        {
            Vector3 newScale = transform.localScale;

            // Increase scale gradually
            newScale.x += growthSpeed * Time.deltaTime;
            newScale.y += growthSpeed * Time.deltaTime;
            newScale.z += growthSpeed * Time.deltaTime;

            transform.localScale = newScale;

            baseDamage = (maxDamage / 1) * (1 / (transform.localScale.x / initialScale.x));

            // Stop growing when target scale is reached
            if (transform.localScale.x >= initialScale.x * targetScale)
            {
                growing = false;
            }
        }

    }

    void OnTriggerStay(Collider _other)
    {
        // Attempt to reference the health script on the collided object
        Health health = _other.gameObject.GetComponentInParent<Health>();

        // If the object has a health script
        if (health != null && health.gameObject.tag != "Player")
        {
            // Deal damage to targets health equal to projectile's damage
            health.TakeDamage(cummulativeDamage, this.transform);
        }
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
    }
}
