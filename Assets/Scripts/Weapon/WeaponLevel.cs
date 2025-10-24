//Author and Contributor: Logan Baysinger
//Purpose: Handles the level mechanics on the weapon and is used by other scripts to read the level.

//Note: This code assumes that the highest level a weapon can be IS the player's level at the time. 
//      So this also assumes that the player's max level dictactes it's max level implictly.

using UnityEngine;

public class WeaponLevel : MonoBehaviour
{
    private Player_Level playerLvl;

    [HideInInspector]
    public int Level { get; private set; }

    //This function attempts to level up the weapon, it enforces the level cannot be higher than the player's level
    //If the level for some reason cannot be increased, it returns false. If the function returns true the level increased.
    public bool LevelUpWeapon()
    {
        bool ret = true;
        //If the weapon level would be greater than the player's level
        if (Level+1 > playerLvl.Level)
        {
            ret = false;
            return ret;
        }
        Level += 1;

        return ret;
    }

    private void Start()
    {
        playerLvl = GameObject.FindAnyObjectByType<Player_Level>();
    }

}
