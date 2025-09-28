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
    [SerializeField] private int reserveAmmo = 20;
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
        IndividualRounds
    }

    [Header("Display Settings")]
    [Tooltip("Reference to TMPro Ammo Counter.")]
    [SerializeField] private TextMeshProUGUI ammoDisplayText;
    #endregion

    #region Getters and Setters
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }

    public bool IsReloading()
    {
        return reloading;
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set current Ammo to maximum
        currentAmmo = ammoCapacity;

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
        }

        // If player's current ammo is greater than 0 and player is not currently reloading, reduce current ammo by 1
        if (currentAmmo > 0 && !reloading)
        {
            // Reduce current ammo by ammoConsumption
            currentAmmo -= ammoConsumption;

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
            default:
                break;
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
                    ammoDisplayText.text = string.Format(currentAmmo + " / " + ammoCapacity + " (Reserve: " + reserveAmmo + ")");
                }
            }
            else
            {
                ammoDisplayText.text = string.Format("Reloading!!! " + currentAmmo + " / " + ammoCapacity);
            }
        }
    }
}
