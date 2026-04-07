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
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogWarning("HEALTH CHEATS - Could not find the player's health");
        }
        if (!cheatsEnabled)
        {
            cheatsEnabled = true;
            health.infiniteHealthOn = true;
        }
        else 
        {
            cheatsEnabled = false;
            health.infiniteHealthOn = false;
        }
    }
}
