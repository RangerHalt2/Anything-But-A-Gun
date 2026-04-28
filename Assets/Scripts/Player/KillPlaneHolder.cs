using UnityEngine;

public class KillPlaneHolder : MonoBehaviour
{
    [SerializeField] private float killLevel = -50f;


    private void Update()
    {
        if(gameObject.transform.position.y < killLevel)
        {
            Health health = gameObject.GetComponent<Health>();
            float maxHealth = health.maxHealth;
            health.TakeDamage(maxHealth, transform);
        }
    }
}
