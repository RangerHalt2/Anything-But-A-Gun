using UnityEngine;

public class SlopPuddleScript : MonoBehaviour
{
    public float puddleTimer;
    [SerializeField] private float baseDamage;
    [SerializeField] private float growthRate = 1.15f;
    [HideInInspector] public float externalDamageMod = 1f;
    [SerializeField] private int teamID = 1;

    void Start() 
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;
        col.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        puddleTimer -= Time.deltaTime;

        if (puddleTimer < 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay(Collider _other)
    {
        // Attempt to reference the health script on the collided object
        Health health = _other.gameObject.GetComponentInParent<Health>();

        // If the object has a health script
        if (health != null && health.teamID != teamID)
        {
            // Deal damage to targets health equal to projectile's damage
            health.TakeDamage(baseDamage * Time.deltaTime, this.transform);
        }
    }
}