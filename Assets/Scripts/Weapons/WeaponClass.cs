using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponClass : MonoBehaviour
{
    public AmmoManager ammoManager;
    public GameObject gunShot;
    public GameObject clickEffect;

    public GameObject[] randomGunShots;

    public float clickCooldown = 0.5f;
    public float clickTimer = 0;
    public GameObject projectilePrefab;
    public float lastFired = Mathf.NegativeInfinity;

    [Header("Key Weapon Components")]
    [SerializeField] private WeaponClass weaponInterface;
    [SerializeField] protected AmmoManager weaponAmmoManager;
    [SerializeField] private GameObject weaponInfoPanel;
    public WeaponLevel weaponLevel;
    public int PTOAmount;

    [HideInInspector] public float cumulativeDamage;
    private WeaponInfoUI infoUI;

    [Header("Weapon \"Flavor\" Settings")]
    [SerializeField] private string weaponName;
    [SerializeField] private string tagline;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private string promotionName = "No Promotion";
    [SerializeField] private string promotionEffect = "No Promotion";
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
    [SerializeField] protected bool hasPackAPunch = false;
    //[SerializeField] private string[] acceptablePackAPunchComponents; 
    [SerializeField] protected PAP_ScriptableObjects[] acceptablePackAPunchOptions;
    protected Type[] components;
    protected int currPackAPunchIndex = -1;
    protected Component currPackAPunchComponent;
    //[HideInInspector] public bool isProjectile { get; set; }
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

    public string GetPromotionName()
    {
        return promotionName;
    }

    public string GetPromotionEffect()
    {
        return promotionEffect;
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

    public int GetPackAPunchLength()
    {
        return acceptablePackAPunchOptions.Length;
    }

    public void SetPackAPunchIndex(int index)
    {
        currPackAPunchIndex = index;
        return;
    }

    public int GetPackAPunchIndex()
    {
        return currPackAPunchIndex;
    }

    public bool AddPackAPunch()
    {
        return AddPackAPunch(currPackAPunchIndex);
    }

    public bool AddPackAPunch(int index)
    {
        Debug.Log("Entered AddPackAPunch by index");
        if (index == -1)
        {
            Debug.LogError("Pack-A-Punch was attempted without having an assigned index");
            return false;
        }
        if(currPackAPunchComponent != null)
        {
            Debug.LogError("Weapon already has a pack-a-punch");
            return false;
        }
        if(components == null || acceptablePackAPunchOptions.Length == 0)
        {
            Debug.LogError("The weapon does not have a valid promotion right now");
            return false;
        }
        Debug.Log("Adding Component " + components[index]);
        currPackAPunchComponent = gameObject.AddComponent(components[index]);

        promotionName = acceptablePackAPunchOptions[index].promotionName;
        promotionEffect = acceptablePackAPunchOptions[index].promotionEffect;

        hasPackAPunch = true;

        if (infoUI != null)
        {
            infoUI.SetInfo(this);
        }

        WeaponHandler wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        if (wh != null)
        {
            if(wh.currentWeapon == this.gameObject)
            {
                wh.weapons.Remove(wh.currentWeapon);
                wh.DropWeapon(wh.currentWeapon);
                wh.EmergencyChooseWeaponZero();
            }
        }

        return true;
    }

    public void RemovePackAPunch()
    {
        if (currPackAPunchComponent == null)
        {
            Debug.Log("Cannot remove currPackAPunch");
            return;
        }
        currPackAPunchIndex = -1;
        promotionName = "No Promotion";
        promotionEffect = "No Promotion";
        currPackAPunchComponent = null;
        hasPackAPunch = false;
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

        /*
        //Debug.LogError("Length " + acceptablePackAPunchComponents.Length);
        components = new Type[acceptablePackAPunchComponents.Length];
        for (int i = 0; i < acceptablePackAPunchComponents.Length; i++)
        {
            components[i] = Type.GetType(acceptablePackAPunchComponents[i]);
        }
        */


        components = acceptablePackAPunchOptions.Select(t => t.GetTypeSafe()).ToArray();

        promotionEffect = "No Promotion";
        promotionName = "No Promotion";

        Debug.Log("WEAPON CLASS - PACK A PUNCH - Array Length: " + acceptablePackAPunchOptions.Length);
        for (var i = 0; i < acceptablePackAPunchOptions.Length; i++)
        {
            Debug.Log("WEAPON CLASS - PACK A PUNCH - component: " + components[i]);
        }
    }

    private void Start()
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
        Debug.Log("WEAPONCLASS - Showing Info");
        if (infoUI != null)
        {
            Debug.Log("WEAPONCLASS - infoUI not null");
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

    public void RandomGunShot(Transform followTrans)
    {
        int num = UnityEngine.Random.Range(0, randomGunShots.Length);
        GameObject selected = randomGunShots[num];
        GameObject randomShot = Instantiate(selected, followTrans.position, Quaternion.identity);
        MovingAudio movingAudio = randomShot.AddComponent<MovingAudio>();
        movingAudio.targetToFollow = followTrans;
    }


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
            else if (ammoManager != null)
            {
                if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
                {
                    ammoManager.ReloadWeapon();
                }
                else
                {
                    if (clickEffect != null && clickTimer <= 0)
                    {
                        clickTimer = clickCooldown;
                        Instantiate(clickEffect, transform.position, transform.rotation, null);
                    }
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
