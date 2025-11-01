using UnityEngine;

public class WeaponCollectScript : MonoBehaviour
{
    //Weapon that will be acquired when this is touched
    private WeaponHandler handler;
    private AmmoManager ammoManager;
    private InputManager inputManager;

    public bool collected = false;

    private void Start()
    {
        inputManager = GameObject.FindAnyObjectByType<InputManager>();
        handler = GameObject.FindAnyObjectByType<WeaponHandler>();
    }

    private void PickUp()
    {
        ammoManager = handler.GetCurrentWeapon().GetComponent<AmmoManager>();
        //LB: If the player is reloading, attempt to cancel the reload and give them the new weapon, if not prevent the execution of code
        if (ammoManager.IsReloading())
        {
            bool cancelled = ammoManager.CancelReload();
            if (!cancelled) return;
        }

        //Telling the Weapon Handler to add this weapon as a new weapon
        handler.newWeapon = gameObject;
        collected = true;
    }

    private void Update()
    {
        InteractTrigger();
    }

    //LB: Checks if the player pressed the input 
    private void InteractTrigger()
    {
        if (!inputManager.InteractInput) return; //The player didn't press the button so why even run code?
        float distance = Vector3.Distance(handler.transform.position, this.transform.position);
        if (distance <= handler.maxDistance) PickUp();
    }

    /* On Collision Pick Up
    void OnTriggerEnter(Collider _other) 
    {
        if (_other.CompareTag("Player") && !collected)
        {
            PickUp();
        }
    }
    */
}
