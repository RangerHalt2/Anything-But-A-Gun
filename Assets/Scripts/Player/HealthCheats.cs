using UnityEngine;

public class HealthCheats : MonoBehaviour
{
    [SerializeField] private Health health;
    public bool cheatsEnabled = false;

    void Start() 
    {
        health = GetComponent<Health>();
    }

    public void ToggleCheats() //Toggling unlimited health and not
    {
        if (!cheatsEnabled)
        {
            cheatsEnabled = true;
            health.currentHealth = Mathf.Infinity;
        }
        else 
        {
            cheatsEnabled = false;
            health.currentHealth = health.maxHealth;
        }
    }
}
