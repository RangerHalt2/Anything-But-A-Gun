using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class WeaponHandler : MonoBehaviour
{
    public GameObject starterWeapon;

    //Exactly what weapons we have
    private List<GameObject> weapons = new List<GameObject>(); //List instead of an array for constant updating
    private GameObject currentWeapon; //The weapon currently in the player's hands
    private int weaponSlot; //Which weapon in slot we're holding
    public GameObject newWeapon; //For if a player picks up a new gun.
    [SerializeField] private Transform weaponLocation; //Where the weapon is in the player's POV

    [SerializeField] private InputManager inputManager;
    private float scrollValue;

    [SerializeField] private Transform gunHolder;

    
    void Start()
    {
        weapons.Add(Instantiate(starterWeapon, gunHolder));
        currentWeapon = weapons[0];
        weaponSlot = 0;
    }

    void Update()
    {
        if (newWeapon != null)
        { AddWeapon(newWeapon); }

        if (currentWeapon != null)
        {
            currentWeapon.transform.position = weaponLocation.position;
        }

        SwitchGun();
    }

    public void FireWeapon()
    {
        currentWeapon.GetComponent<IWeapon>().Shoot();
    }

    public void ReloadWeapon() 
    { 
        currentWeapon.GetComponent<IWeapon>().Reload();
    }

    void SwitchGun() //When the player want's to rotate to a different weapon in their wheel
    {
        if (inputManager != null)
        {
            scrollValue = inputManager.scrollAction.ReadValue<float>();
        }

        if (weapons.Count > 1 && scrollValue != 0 && !currentWeapon.GetComponent<AmmoManager>().IsReloading())
        {
            currentWeapon.SetActive(false); //Deactivate (not destroy) current weapon
        }
        else
        {
            return;
        }

        //If player scrolls
        if (scrollValue > 0 && !currentWeapon.GetComponent<AmmoManager>().IsReloading()) //Scroll up
        {
            weaponSlot++;

            if (weaponSlot >= weapons.Count) //Actually check if it's a gun. If not, change back to first gun
            {
                weaponSlot = 0;
                currentWeapon = weapons[weaponSlot];
                currentWeapon.GetComponent<AmmoManager>().updateDisplay();
            }
            else
            {
                currentWeapon = weapons[weaponSlot]; //Set current weapon to next weapon
                currentWeapon.GetComponent<AmmoManager>().updateDisplay();
            }
            
            //Play equip animation and activate new current weapon
        }
        else if (scrollValue < 0 && !currentWeapon.GetComponent<AmmoManager>().IsReloading()) //Scroll down
        {
            weaponSlot--;
            if (weaponSlot < 0) //Actually check if it's a gun. If not, change to base
            {
                weaponSlot = weapons.Count - 1;
                currentWeapon = weapons[weaponSlot];
                currentWeapon.GetComponent<AmmoManager>().updateDisplay();
            }
            else
            {
                currentWeapon = weapons[weaponSlot]; //Set current weapon to next weapon
                currentWeapon.GetComponent<AmmoManager>().updateDisplay();
            }

            //Play equip animation and activate new current weapon
            
        }

        if (currentWeapon.activeSelf == false)
        {
            currentWeapon.SetActive(true); //And activate it
            Debug.Log(currentWeapon);
        }

    }

    void AddWeapon(GameObject addWeapon)
    {
        if (weapons != null)
        {
            currentWeapon.SetActive(false);
            weapons.Add(Instantiate(addWeapon, gunHolder)); //Adds the weapon to the list of weapons available.
            weaponSlot = weapons.Count - 1;
            currentWeapon = weapons[weaponSlot]; //Create the actual weapon and make it active
            currentWeapon.SetActive(true);
            //currentWeapon.GetComponent<AmmoManager>().updateDisplay();
            newWeapon = null; //So that it doesn't endlessly add the same weapon.
            
        }
    }
}
