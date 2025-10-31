using UnityEngine;

public class WeaponCollectScript : MonoBehaviour, IInteractable
{
    //Weapon that will be acquired when this is touched
    private AmmoManager ammoManager;

    public bool collected = false;

    /*void OnTriggerEnter(Collider _other) 
    {
        if (_other.CompareTag("Player") && !collected)
        {
            WeaponHandler handler = _other.GetComponentInParent<WeaponHandler>();
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
    }*/

    public void Interact()
    {
        WeaponHandler handler = PlayerController.Instance.GetComponent<WeaponHandler>();
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

        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
