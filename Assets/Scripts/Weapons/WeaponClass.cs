using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WeaponClass : MonoBehaviour
{
    public AmmoManager ammoManager;
    public GameObject gunShot;
    public GameObject projectilePrefab;
    public float lastFired = Mathf.NegativeInfinity;

    [Header("Key Weapon Components")]
    [SerializeField] private WeaponClass weaponInterface;
    [SerializeField] private AmmoManager weaponAmmoManager;
    [SerializeField] private GameObject weaponInfoPanel;
    public WeaponLevel weaponLevel;

    [HideInInspector] public float cumulativeDamage;
    private WeaponInfoUI infoUI;

    [Header("Weapon \"Flavor\" Settings")]
    [SerializeField] private string weaponName;
    [SerializeField] private string tagline;
    [SerializeField] private WeaponType weaponType;
    [Space]
    public int level;
    [Header("Damage Settings")]
    public float baseDamage;
    public float levelDamage;
    public float fireRate;

    [Header("Ammo Settings")]
    [SerializeField] private float reloadTime;
    [SerializeField] private int AmmoCapacity;
    [SerializeField] private int reserveAmmo;

    [Header("Pack-A-Punch Related Stuff")]
    [SerializeField] private bool hasPackAPunch = false;

    public enum WeaponType
    {
        SemiAutomatic,
        FullyAutomatic,
        Charge,
        Beam,
        Heat,
        Melee,
        Thrown
    }

    #region WeaponDataGetters
    public string GetWeaponName()
    {
        return weaponName;
    }

    public string GetWeaponTagline()
    {
        return tagline;
    }

    public int GetWeaponLevel()
    {
        return level;
    }

    public float GetBaseDamage()
    {
        return baseDamage;
    }
    public float GetLevelDamage()
    {
        return levelDamage;
    }

    // Returns the selected Weapon Type as a String
    public string GetWeaponTypeAsString()
    {
        switch (weaponType)
        {
            case WeaponType.SemiAutomatic:
                return "Semi-Auto";
            case WeaponType.FullyAutomatic:
                return "Full-Auto";
            case WeaponType.Charge:
                return "Charge";
            case WeaponType.Beam:
                return "Beam";
            case WeaponType.Heat:
                return "Heat";
            case WeaponType.Melee:
                return "Melee";
            case WeaponType.Thrown:
                return "Thrown";
            default:
                return "Unkown";
        }
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public float GetReloadTime()
    {
        return reloadTime;
    }

    public int GetAmmoCapacity()
    {
        return AmmoCapacity;
    }

    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }
    #endregion

    #region Weapon
    private void Awake()
    {
        if (weaponInfoPanel != null)
        {
            // Find the reference to the weaponInfoUI script
            infoUI = weaponInfoPanel.GetComponent<WeaponInfoUI>();
            // Disable the info panel on startup
            weaponInfoPanel.SetActive(false);
        }

        InitializeWeapon();
    }

    private void InitializeWeapon()
    {
        weaponLevel = GetComponent<WeaponLevel>();
        //if (weaponData == null) return;

        CalculateLevelDamage();

        //weaponInterface.fireRate = weaponData.GetFireRate();

        // Initialize weapon Ammo Manager
        weaponAmmoManager.SetReloadTime(GetReloadTime());
        weaponAmmoManager.SetAmmoCapacity(GetAmmoCapacity());
        weaponAmmoManager.SetCurrentAmmo(GetAmmoCapacity());
        weaponAmmoManager.SetReserveAmmo(GetReserveAmmo());
    }

    public void ShowInfo()
    {
        if (infoUI != null)
        {
            // Update Weapon Info
            infoUI.SetInfo(GetComponent<WeaponClass>());
            // Activate the panel
            weaponInfoPanel.SetActive(true);
        }
    }

    public void HideInfo()
    {
        if (weaponInfoPanel != null)
        {
            // Deactivate the panel
            weaponInfoPanel.SetActive(false);
        }
    }

    public void CalculateLevelDamage()
    {
        Debug.Log(baseDamage);
        Debug.Log(levelDamage);
        Debug.Log(weaponLevel);
        cumulativeDamage = GetBaseDamage() + (GetLevelDamage() * (weaponLevel.Level - 1));
        //Debug.Log("Weapon: Cumulative Damage " + cumulativeDamage);
    }
    #endregion

    #region BasicWeaponFunction
    public virtual void Shoot() //Default is spawn projectile
    {
        // If enough time has passed since the last round was fired
        if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
        {
            // If there is an assigned ammo manager, and that ammo manager has at least one round of ammo loaded
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
            {
                // Attempt to fire the weapon
                ammoManager.Fire();
                // If the weapon is not reloading
                if (!ammoManager.IsReloading())
                {

                    if (projectilePrefab != null)
                    {
                        SpawnProjectile();
                    }
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;

                }
            }

        }
    } //"Virtual" allows children to override it
    public virtual void Reload()
    {
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            ammoManager.ReloadWeapon();
        }
    }

    public virtual void SpawnProjectile() { }
    #endregion
}
