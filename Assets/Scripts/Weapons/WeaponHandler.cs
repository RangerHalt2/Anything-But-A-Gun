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
    [SerializeField] public float maxDistance = 5;

    [SerializeField] private InputManager inputManager;
    private float scrollValue;
    [SerializeField] private float weaponSwitchRate = 0.3f;
    private float lastSwitch = Mathf.NegativeInfinity;

    [SerializeField] private Transform gunHolder;

    //LB: Added this as a getter
    public GameObject GetCurrentWeapon() { return currentWeapon;}

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
        else
        {
            Debug.LogWarning("InputManager is not assigned.");
            return;
        }

        if (currentWeapon == null)
        {
            Debug.LogError("CurrentWeapon is null — likely no starter weapon assigned.");
            return;
        }

        AmmoManager ammo = currentWeapon.GetComponent<AmmoManager>();
        if (ammo == null)
        {
            Debug.LogError($"Weapon {currentWeapon.name} has no AmmoManager component.");
            return;
        }

        // If enough time has passed since the last round was fired

        if ((Time.timeSinceLevelLoad - lastSwitch) > weaponSwitchRate)
        {
            if (weapons.Count > 1 && (scrollValue != 0 || inputManager.NextInput != 0) && !currentWeapon.GetComponent<AmmoManager>().IsReloading())
            {
                currentWeapon.SetActive(false); //Deactivate (not destroy) current weapon
            }
            else
            {
                return;
            }


            //If player scrolls
            if ((scrollValue > 0 || inputManager.NextInput > 0f) && !currentWeapon.GetComponent<AmmoManager>().IsReloading()) //Scroll up
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
            else if ((scrollValue < 0 || inputManager.NextInput < 0f) && !currentWeapon.GetComponent<AmmoManager>().IsReloading()) //Scroll down
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
            
            lastSwitch = Time.timeSinceLevelLoad;
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
            //Check if the weapon we're trying to pick up is of a type we already have

            IWeapon newWeaponType = addWeapon.GetComponent<IWeapon>();

            System.Type newScriptName = newWeaponType.GetType();
            string nameOfNewScript = newScriptName.Name;

            currentWeapon.SetActive(false);

            if (nameOfNewScript != null)
            {
                foreach (GameObject heldWeapon in weapons)
                {
                    IWeapon currentWeaponType = heldWeapon.GetComponent<IWeapon>();

                    System.Type heldScriptName = currentWeaponType.GetType();
                    string nameOfHeldScript = heldScriptName.Name;

                    if (nameOfHeldScript == nameOfNewScript) //If we already have this weapon, drop the current weapon and replace it with the new one.
                    { 
                        weapons.Remove(heldWeapon);
                        DropWeapon(heldWeapon);
                        break;
                    }

                }

                weapons.Add(addWeapon); //Adds the weapon to the list of weapons available.
                weaponSlot = weapons.Count - 1;
                currentWeapon = weapons[weaponSlot]; //Make the current weapon the new weapon we just added

                // RL: Check if there is a Weapon Transform Manager
                WeaponTransformManager wtm = currentWeapon.GetComponentInChildren<WeaponTransformManager>();
                // If so use it to update the Weapon's scale
                if (wtm != null)
                {
                    currentWeapon.transform.SetParent(weaponLocation, false);
                    currentWeapon.transform.localPosition = Vector3.zero;
                    currentWeapon.transform.localRotation = Quaternion.identity;
                    wtm.SetEquipped();
                }
                // If not, use the old system
                else
                {
                    Vector3 scale = currentWeapon.transform.localScale;
                    currentWeapon.transform.SetParent(weaponLocation, true); //"True" refers to the world position of the object remaining true (look up worldPositionStays) making sure it's not scaled weird
                    currentWeapon.transform.position = gunHolder.transform.position;
                    currentWeapon.transform.rotation = gunHolder.transform.rotation;
                    currentWeapon.transform.localScale = scale;
                }
                
                currentWeapon.GetComponent<WeaponCollectScript>().enabled = false;
                currentWeapon.SetActive(true);
                currentWeapon.GetComponent<AmmoManager>().updateDisplay();
                //currentWeapon.GetComponent<AmmoManager>().updateDisplay();
                newWeapon = null; //So that it doesn't endlessly add the same weapon.
            }
        }
    }

    void DropWeapon(GameObject dropWeapon) //Spawns a collectible that will allow you to recollect the weapon.
    {
        Vector3 spawnPoint = transform.position + transform.forward * 2;
        Vector3 scale = dropWeapon.transform.localScale;
        //Spawn the collectible in front of the player
        dropWeapon.transform.SetParent(null, true);
        dropWeapon.transform.localScale = scale;
        dropWeapon.transform.position = spawnPoint;
        dropWeapon.SetActive(true);


        dropWeapon.GetComponent<WeaponCollectScript>().enabled = true;
        dropWeapon.GetComponent<WeaponCollectScript>().collected = false; //Currently, if you drop the weapon and pick it up too fast, there's a chance to scramble
                                                                          //it's collected function and make it uncollectible. This won't be an issue if we change to
                                                                          //interactive pick-ups.
                                                                          // Check if there is a Weapon Transform Manager
        WeaponTransformManager wtm = dropWeapon.GetComponentInChildren<WeaponTransformManager>();
        // If so use it to update the weapon's scale
        if (wtm != null)
        {
            dropWeapon.transform.localRotation = Quaternion.identity;
            wtm.SetGrounded();
        }
        // If not use the old system
        else
        {
            dropWeapon.transform.rotation = Quaternion.identity;
        }


        dropWeapon.layer = LayerMask.NameToLayer("Interactable");

        dropWeapon.GetComponentInChildren<FloatAndRotate>().StartFloatAndRotate();

    }
}
