using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public GameObject starterWeapon;

    //Exactly what weapons we have
    public List<GameObject> weapons = new List<GameObject>(); //List instead of an array for constant updating
    public GameObject currentWeapon; //The weapon currently in the player's hands
    public GameObject newWeapon; //For if a player picks up a new gun.
    public Transform weaponLocation; //Where the weapon is in the player's POV

    void Start()
    {
        weapons.Add(starterWeapon);
        currentWeapon = Instantiate(weapons[0]);
    }

    void Update() 
    {
        if (newWeapon != null)
        { AddWeapon(newWeapon); }
        currentWeapon.transform.position = weaponLocation.position;

        Scroll();
    }

    void Scroll() //When the player want's to rotate to a different weapon in their wheel
    { 
        //If player scrolls
        //Deactivate (not destroy) current weapon
        //Set current weapon to next weapon
        //Play equip animation and activate new current weapon
    }

    void AddWeapon(GameObject addWeapon)
    {
        if (weapons != null)
        {
            currentWeapon.SetActive(false);
            weapons.Add(addWeapon); //Adds the weapon to the list of weapons available.
            currentWeapon = Instantiate(addWeapon); //Create the actual weapon and make it active
            currentWeapon.SetActive(true);
            newWeapon = null; //So that it doesn't endlessly add the same weapon.

            //For debugging to make sure the list is updated
            foreach (GameObject item in weapons)
            {
                Debug.Log(item);
            }
        }
    }
}
