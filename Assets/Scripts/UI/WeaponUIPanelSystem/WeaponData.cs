using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon \"Flavor\" Settings")]
    [SerializeField] private string weaponName;
    [SerializeField] private string tagline;
    [SerializeField] private WeaponType weaponType;
    [Space]
    [SerializeField] private int level;
    [Header("Damage Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;

    [Header("Ammo Settings")]
    [SerializeField] private float reloadTime;
    [SerializeField] private int AmmoCapacity;
    [SerializeField] private int reserveAmmo;


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

    #region Getters
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
}
