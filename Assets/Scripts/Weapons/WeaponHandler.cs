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
    public List<GameObject> weapons = new List<GameObject>(); //List instead of an array for constant updating
    public GameObject currentWeapon; //The weapon currently in the player's hands
    public int weaponSlot; //Which weapon in slot we're holding
    public GameObject newWeapon; //For if a player picks up a new gun.
    public Transform weaponLocation; //Where the weapon is in the player's POV

    public InputManager inputManager;
    public float scrollValue;

    void Awake() 
    {
        inputManager = new InputManager();
    }
    void OnEnable()
    {
        inputManager.scrollAction.performed += OnSwitchGun;
    }
    void OnDisable() 
    {
        inputManager.scrollAction.performed -= OnSwitchGun;
    }

    void Start()
    {
        weapons.Add(starterWeapon);
        currentWeapon = Instantiate(weapons[0]);
        weaponSlot = 0;
    }

    

    void Update()
    {
        if (newWeapon != null)
        { AddWeapon(newWeapon); }
        currentWeapon.transform.position = weaponLocation.position;
    }

    //NOT WORKING! IDK WHY
    void OnSwitchGun(InputAction.CallbackContext ctx) //When the player want's to rotate to a different weapon in their wheel
    {
        scrollValue = ctx.ReadValue<float>();
        Debug.Log("Mouse Scrolled");

        //If player scrolls
        if (scrollValue > 0) //Scroll up
        {
            currentWeapon.SetActive(false); //Deactivate (not destroy) current weapon
            if (weaponSlot > weapons.Count - 1) //Actually check if it's a gun. If not, change back to first gun
            {
                currentWeapon = weapons[0];
                weaponSlot = 0;
            }
            else
            {
                currentWeapon = weapons[weaponSlot + 1]; //Set current weapon to next weapon
            }
            currentWeapon.SetActive(true); //And activate it
            //Play equip animation and activate new current weapon
        }
        else if (scrollValue < 0) //Scroll down
        {
            currentWeapon.SetActive(false); //Deactivate (not destroy) current weapon
            if (weaponSlot < 0) //Actually check if it's a gun. If not, change to base
            {
                currentWeapon = weapons[weapons.Count];
                weaponSlot = weapons.Count - 1;
            }
            else
            {
                currentWeapon = weapons[weaponSlot - 1]; //Set current weapon to next weapon
            }
            currentWeapon.SetActive(true); //And activate it
            //Play equip animation and activate new current weapon
        }
        
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
            weaponSlot = weapons.Count - 1;
            Debug.Log(weaponSlot);

            //For debugging to make sure the list is updated
            foreach (GameObject item in weapons)
            {
                Debug.Log(item);
            }
        }
    }
}
