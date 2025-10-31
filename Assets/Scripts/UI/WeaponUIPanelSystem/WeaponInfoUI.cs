using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Build.Content;
using System;

public class WeaponInfoUI : MonoBehaviour
{
    [Header("Weapon Component References")]
    [SerializeField] private AmmoManager ammoManager;
    private IWeapon weaponInterface;

    [Header("TMPro References")]
    [Tooltip("Reference to the TMPro object which displays the weapon's name.")]
    [SerializeField] private TMP_Text nameText;
    [Tooltip("Reference to the TMPro object which displays the weapon's level.")]
    [SerializeField] private TMP_Text levelText;
    [Tooltip("Reference to the TMPro object which displays type of weapon.")]
    [SerializeField] private TMP_Text typeText;
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
    [Tooltip("Reference to the TMPro object which displays the weapon's tagline/description.")]
    [SerializeField] private TMP_Text taglineText;
    
    public void SetInfo(WeaponData data)
    {
        nameText.text = data.GetWeaponName();

        levelText.text = "Lvl. " + data.GetWeaponLevel().ToString();

        typeText.text = data.GetWeaponTypeAsString();


        // StatsVal
        //damageText.text = "Damage: " + data.damage;

        // Currently there is no way to actually get the fire rate off of the weapon components. Scriptable Object data may be innacurate
        fireRateText.text = data.GetFireRate().ToString() + "s";

        // Ammo
        reloadSpeedText.text = ammoManager.GetReloadTime().ToString() + "s";
        magazineSizeText.text = ammoManager.GetAmmoCapacity().ToString();
        reserveAmmoText.text = ammoManager.GetReserveAmmo().ToString();

        taglineText.text = data.GetWeaponTagline();
    }
}