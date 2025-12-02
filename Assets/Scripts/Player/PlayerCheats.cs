using UnityEngine;

public class PlayerCheats : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private InputManager inputs;
    [SerializeField] private AmmoManager weapons;
    public bool cheatsEnabled = false;

    void Start() 
    {
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (inputs.CheatsInput)
        {
            if (!cheatsEnabled)
            {
                EnableCheats();
            }
            else 
            {
                DisableCheats();
            }
        }
        
    }
    void EnableCheats() 
    {
        cheatsEnabled = true;
        weapons = GetComponent<WeaponHandler>().currentWeapon.GetComponent<AmmoManager>();
        weapons.reserveAmmo = -1;
        health.currentHealth = Mathf.Infinity;
    }
    void DisableCheats() 
    {
        cheatsEnabled = false;
        weapons.reserveAmmo = 20;
        health.currentHealth = health.maxHealth;
    }
}
