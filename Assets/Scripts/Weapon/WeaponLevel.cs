using UnityEngine;

public class WeaponLevel : MonoBehaviour
{
    private Player_Level plyrLvl;

    [HideInInspector]
    public int Level { get; private set; }

    //This function attempts to level up the weapon, it enforces the level cannot be higher than the player's level
    //If the level for some reason cannot be increased, it returns false. If the function returns true the level increased.
    public bool LevelUpWeapon()
    {
        bool ret = true;
        //If the weapon level would be greater than the player's level
        if (Level+1 > plyrLvl.Level)
        {
            ret = false;
            return ret;
        }
        Level += 1;

        return ret;
    }

    private void Start()
    {
        plyrLvl = GetComponent<Player_Level>();
    }

}
