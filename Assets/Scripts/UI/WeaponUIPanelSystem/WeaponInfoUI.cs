using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class WeaponInfoUI : MonoBehaviour
{
    [Header("Weapon Component References")]
    [SerializeField] private AmmoManager ammoManager;
    [SerializeField] private WeaponLevel weaponLevel;
    [SerializeField] private Weapon weapon;

    [Header("TMPro \"Flavor\" References")]
    [Tooltip("Reference to the TMPro object which displays the weapon's name.")]
    [SerializeField] private TMP_Text nameText;
    [Tooltip("Reference to the TMPro object which displays the weapon's level.")]
    [SerializeField] private TMP_Text levelText;
    [Tooltip("Reference to the TMPro object which displays type of weapon.")]
    [SerializeField] private TMP_Text typeText;
    [Tooltip("Reference to the TMPro object which displays the weapon's tagline/description.")]
    [SerializeField] private TMP_Text taglineText;
    [Tooltip("Reference to the TMPro object which displays the weapon's damage.")]
    [SerializeField] private TMP_Text damageText;
    [Tooltip("Reference to the TMPro object which displays the weapon's fire rate.")]
    [SerializeField] private TMP_Text fireRateText;
    [Tooltip("Reference to the TMPro object which displays the weapon's reload speed.")]
    [SerializeField] private TMP_Text reloadSpeedText;
    [Tooltip("Reference to the TMPro object which displays the weapon's magazine size.")]
    [SerializeField] private TMP_Text magazineSizeText;
    [Tooltip("Reference to the TMPro object which displays the weapon's reserve ammo count.")]
    [SerializeField] private TMP_Text reserveAmmoText;

    [Header("TMPro StatVal References")]
    [Tooltip("Reference to the TMPro object which displays the weapon's damage.")]
    [SerializeField] private TMP_Text damageTextVal;
    [Tooltip("Reference to the TMPro object which displays the weapon's fire rate.")]
    [SerializeField] private TMP_Text fireRateTextVal;
    [Tooltip("Reference to the TMPro object which displays the weapon's reload speed.")]
    [SerializeField] private TMP_Text reloadSpeedTextVal;
    [Tooltip("Reference to the TMPro object which displays the weapon's magazine size.")]
    [SerializeField] private TMP_Text magazineSizeTextVal;
    [Tooltip("Reference to the TMPro object which displays the weapon's reserve ammo count.")]
    [SerializeField] private TMP_Text reserveAmmoTextVal;

    [Header("StatTypes References")]
    [Tooltip("Reference to the TMPro object which displays the weapon's damage.")]
    [SerializeField] private GameObject damage;
    [Tooltip("Reference to the TMPro object which displays the weapon's fire rate.")]
    [SerializeField] private GameObject fireRate;
    [Tooltip("Reference to the TMPro object which displays the weapon's reload speed.")]
    [SerializeField] private GameObject reloadSpeed;
    [Tooltip("Reference to the TMPro object which displays the weapon's magazine size.")]
    [SerializeField] private GameObject magazineSize;
    [Tooltip("Reference to the TMPro object which displays the weapon's reserve ammo count.")]
    [SerializeField] private GameObject reserveAmmo;
    
    void Awake()
    {
        if (weapon == null)
        weapon = GetComponentInParent<Weapon>();

        if (weapon != null)
        {
            WeaponData weaponData = weapon.GetWeaponData();
            ConfigureStatTypes(weaponData);
        }
    }

    public void SetInfo(WeaponData data)
    {
        nameText.text = data.GetWeaponName();

        levelText.text = "Lvl. " + weaponLevel.Level.ToString();

        typeText.text = data.GetWeaponTypeAsString();


        // StatsVal
        if (weapon != null)
        {
            weapon.CalculateLevelDamage();
            damageTextVal.text = weapon.cumulativeDamage.ToString();
        }

        // Currently there is no way to actually get the fire rate off of the weapon components. Scriptable Object data may be innacurate
        fireRateTextVal.text = data.GetFireRate().ToString() + "s";

        // Ammo
        reloadSpeedTextVal.text = ammoManager.GetReloadTime().ToString() + "s";
        magazineSizeTextVal.text = ammoManager.GetAmmoCapacity().ToString();
        if (ammoManager.GetReserveAmmo() == -1)
        {
            reserveAmmoTextVal.text = "Infinite";
        }
        else
        {
            reserveAmmoTextVal.text = ammoManager.GetReserveAmmo().ToString();
        }

        taglineText.text = data.GetWeaponTagline();
    }

    private void ConfigureStatTypes(WeaponData data)
    {
        // If the weapon is a Melee Weapon
        if(String.Compare(data.GetWeaponTypeAsString(), "Melee") == 0)
        {
            // Disable Reserve ammo and reload Speed
            reserveAmmo.SetActive(false);
            reloadSpeed.SetActive(false);
            // Change "Fire Rate" to "Swing Speed"
            fireRateText.text = "Swing Speed";
            // Change "Magazine Size" to "Durability"
            magazineSizeText.text = "Durability";

        }
    }
}