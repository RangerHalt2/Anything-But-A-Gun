using UnityEngine;

public class WeaponCollectScript : MonoBehaviour
{
    //Weapon that will be acquired when this is touched
    public GameObject definedWeapon;

    void OnTriggerEnter(Collider _other) 
    {
        if (_other.tag == "Player")
        {
            WeaponHandler handler = _other.GetComponent<WeaponHandler>();

            //Telling the Weapon Handler to add this weapon as a new weapon
            handler.newWeapon = definedWeapon;

            Destroy(gameObject);
        }
    }
}
