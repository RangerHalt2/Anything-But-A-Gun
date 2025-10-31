using System.Data.Common;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private WeaponData weaponData;

    [Header("Key Weapon Components")]
    [SerializeField] private IWeapon weaponInterface;
    [SerializeField] private AmmoManager weaponAmmoManager;
    [SerializeField] private GameObject weaponInfoPanel;

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
}
