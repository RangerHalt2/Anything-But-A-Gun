using UnityEngine;

public class WeaponCollectScript : MonoBehaviour
{
    //Weapon that will be acquired when this is touched
    public GameObject definedWeapon;

    private AmmoManager ammoManager;

    void OnTriggerEnter(Collider _other) 
    {
        if (_other.tag == "Player")
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
            handler.newWeapon = definedWeapon;

            Destroy(gameObject);
        }
    }
}
