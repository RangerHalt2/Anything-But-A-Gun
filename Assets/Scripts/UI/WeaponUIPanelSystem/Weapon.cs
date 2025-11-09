using System.Data.Common;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private WeaponClass weaponData;
    public WeaponClass GetWeaponData() => weaponData;

    [Header("Key Weapon Components")]
    [SerializeField] private WeaponClass weaponInterface;
    [SerializeField] private AmmoManager weaponAmmoManager;
    [SerializeField] private GameObject weaponInfoPanel;
    [SerializeField] private WeaponLevel weaponLevel;

    [HideInInspector] public float cumulativeDamage;
    private WeaponInfoUI infoUI;

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
        if (weaponData == null) return;

        CalculateLevelDamage();

        //weaponInterface.fireRate = weaponData.GetFireRate();

        // Initialize weapon Ammo Manager
        weaponAmmoManager.SetReloadTime(weaponData.GetReloadTime());
        weaponAmmoManager.SetAmmoCapacity(weaponData.GetAmmoCapacity());
        weaponAmmoManager.SetCurrentAmmo(weaponData.GetAmmoCapacity());
        weaponAmmoManager.SetReserveAmmo(weaponData.GetReserveAmmo());
    }

    public void ShowInfo()
    {
        if (infoUI != null)
        {
            // Update Weapon Info
            infoUI.SetInfo(weaponData);
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
        cumulativeDamage = weaponData.GetBaseDamage() + (weaponData.GetLevelDamage() * (weaponLevel.Level - 1));
        //Debug.Log("Weapon: Cumulative Damage " + cumulativeDamage);
    }
}
