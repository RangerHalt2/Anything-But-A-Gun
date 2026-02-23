// Created By: Ryan Lupoli
// This script is meant to track and manage ammunition for a variety of weapons.
using System.Collections;
using UnityEngine;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    #region Variables
    [Header("Ammo Settings")]
    [Tooltip("The amount of ammo the player has in reserve. Set to -1 for infinite reserve ammo")]
    public int reserveAmmo = 20; //EW: Made public to enable cheats
    [Tooltip("The maximum amount of ammo the weapon can have loaded at once.")]
    [SerializeField] private int ammoCapacity = 10;
    [Tooltip("The current amount of ammo the weapon can has loaded.")]
    [SerializeField] private int currentAmmo = 10;
    [Tooltip("The amount of ammo the weapon consumes per \"shot\".")]
    [SerializeField] private int ammoConsumption = 1;

    [Header("Reload Settings")]
    [Tooltip("The amount of time (in seconds) it takes for a weapon to reload.")]
    [SerializeField] private float reloadTime = 1f;
    // The amount of time which has passed since the player started the reloading process
    private float elapsedReloadTime;
    // Tracks whether or not the player is currently reloading
    private bool reloading = false;
    [Tooltip("Determines whether or not the reloading process may be cancelled.")]
    [SerializeField] private bool canCancelReload = false;
    [Tooltip("Determines how does the gun reload.")]
    public ReloadMode reloadMode;

    // Enum of potential Wall Interactions
    public enum ReloadMode
    {
        Magazine,
        IndividualRounds,
        Recharge //EW: Added specifically for the jousting horse
    }

    [Header("Display Settings")]
    [Tooltip("Reference to the player's crosshair.")]
    [SerializeField] private GameObject crosshair;
    [Tooltip("Reference to TMPro Ammo Counter.")]
    private GameObject ammoCounter;
    [SerializeField] private TextMeshProUGUI ammoDisplayText;

    [Header("FX Settings")]
    [Tooltip("Reference to a prefab for an effect which triggers when the player reloads their weapon.")]
    public GameObject reloadEffect;

    [Header("Debug Settings")]
    [Tooltip("Prevents ammuntion from being deducted from the magazine when enabled. Allows for infinite ammo without needing to reload.")]
    [SerializeField] private bool bottomlessMagazines;

    [Header("Reload Animations Stuff")]
    [SerializeField] private ParticleSystem reloadVFX;
    [SerializeField] private MeshRenderer weaponObject;
    #endregion

    #region Getters and Setters
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
    public void SetCurrentAmmo(int newCurrentAmmo)
    {
        currentAmmo = newCurrentAmmo;
    }
    public int GetAmmoCapacity()
    {
        return ammoCapacity;
    }
    public void SetAmmoCapacity(int newAmmoCapacity)
    {
        currentAmmo = newAmmoCapacity;
    }
    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }
    public void SetReserveAmmo(int newReserveAmmo)
    {
        reserveAmmo = newReserveAmmo;
    }
    public float GetReloadTime()
    {
        return reloadTime;
    }
    public void SetReloadTime(float newReloadTime)
    {
        reloadTime = newReloadTime;
    }
    public bool IsReloading()
    {
        return reloading;
    }

    public int GetAmmoConsumption()
    {
        return ammoConsumption;
    }

    //LB: Added a function to cancel the reload, it returns true if the reload is cancelled and false if it cannot be cancelled
    public bool CancelReload()
    {
        bool ret = true;
        if(canCancelReload)
            StopCoroutine(Reload());
        else
            ret = false;

        return ret;
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set current Ammo to maximum
        currentAmmo = ammoCapacity;
        ammoDisplayText = GameObject.FindGameObjectWithTag("AmmoCounter").GetComponent<TextMeshProUGUI>();

        updateDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        // Cancel the reloading process if the player shoots while in an individual rounds reload process
        if (reloading && canCancelReload)
        {
            reloading = false;
            StopCoroutine(Reload()); // Stop the reload coroutine
            if (crosshair != null)
            {
                crosshair.SetActive(true);
            }
        }

        // If player's current ammo is greater than 0 and player is not currently reloading, reduce current ammo by 1
        if (currentAmmo > 0 && !reloading)
        {
            if (!bottomlessMagazines)
            {
                // Reduce current ammo by ammoConsumption
                currentAmmo -= ammoConsumption;
            }
            
            updateDisplay();
            return;
        }
        // If player has no ammo, do nothing
        else
        {
            Debug.Log(gameObject.name + "is out of ammo and needs to reload!");
            return;
        }
    }

    public void ReloadWeapon()
    {
        if (!reloading && currentAmmo != ammoCapacity)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (reloadVFX != null)
            reloadVFX.Play();
        if (weaponObject != null)
            weaponObject.enabled = false;
        // Disable the player's crosshair for the duration of the reloading process
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        // Play the reloading Effect for the weapon
        if (reloadEffect != null)
        {
            Instantiate(reloadEffect, transform.position, transform.rotation, null);
        }

        switch (reloadMode)
            {
                // Reload method for weapons that use a "Magazine"
                case ReloadMode.Magazine:
                    // Set the reloading bool to true
                    reloading = true;
                    
                    updateDisplay();

                    yield return new WaitForSeconds(reloadTime);

                    // If reloading has not been cancelled
                    if (reloading)
                    {
                        // If the player has infinite reserve ammo
                        if (reserveAmmo == -1)
                        {
                            // Set current ammo to the capacity
                            currentAmmo = ammoCapacity;
                        }
                        // Else if the player currently has more than enough ammo in reserve to fully reload...
                        else if (reserveAmmo >= ammoCapacity)
                        {
                            // Fully reload their magazine
                            currentAmmo = ammoCapacity;
                            // Reduce reserve ammo by the capacity of the weapon
                            reserveAmmo -= ammoCapacity;
                        }
                        // If the player does not have enough ammo to fully reload their weapon
                        else
                        {
                            // Set current ammo to the amount of ammo in reserve
                            currentAmmo = reserveAmmo;
                            // Set the reserve Ammo to 0
                            reserveAmmo = 0;
                        }
                    }
                    // Set reloading to false
                    reloading = false;
                    if(reloadVFX != null)
                        reloadVFX.Stop();
                    if(weaponObject != null)
                        weaponObject.enabled = true;
                    updateDisplay();
                    break;
                // Reload method for weapons that reload each round individually
                case ReloadMode.IndividualRounds:
                    // Set reloading bool to true
                    reloading = true;
                    updateDisplay();

                    while (currentAmmo < ammoCapacity && (reserveAmmo > 0 || reserveAmmo == -1))
                    {
                        // Wait for reload time
                        yield return new WaitForSeconds(reloadTime);

                        // Check if reloading has been canceled
                        if (!reloading)
                        {
                            break;
                        }

                        // Add one round to the player's current ammo
                        currentAmmo += 1;

                        if (reserveAmmo > 0)
                        {
                            reserveAmmo -= 1;
                        }

                        updateDisplay();
                    }
                    // Set reloading to false
                    reloading = false;
                    updateDisplay();
                    break;

            case ReloadMode.Recharge:
                // Set reloading bool to true
                reloading = true;
                updateDisplay();

                    // Wait for reload time
                    yield return new WaitForSeconds(reloadTime);

                    // Add one round to the player's current ammo
                    currentAmmo += 1;

                    if (reserveAmmo > 0)
                    {
                        reserveAmmo -= 1;
                    }

                    updateDisplay();
               
                // Set reloading to false
                reloading = false;
                updateDisplay();
                break;
            default:
                break;
        }
        // Re-enable the crosshair once the reload has been completed
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

    public void updateDisplay()
    {
        // Used if there is a TMPro Display assinged
        if (ammoDisplayText != null)
        {
            if (!reloading)
            {
                if (reserveAmmo == -1)
                {
                    ammoDisplayText.text = string.Format(currentAmmo + " / " + ammoCapacity + " (Reserve: Infinite)");
                }
                else
                {
                    ammoDisplayText.text = string.Format(currentAmmo+"");
                }
            }
            else
            {
                //ammoDisplayText.text = string.Format("Reloading!!! " + currentAmmo + " / " + ammoCapacity);
            }
        }
    }
}
