using UnityEngine;
using UnityEngine.UI;

public class WeaponCheatHelper : MonoBehaviour
{
    [SerializeField] private Toggle cheat;


    private void OnEnable()
    {
        DoorIdentifier[] allDoors = GameObject.FindObjectsByType<DoorIdentifier>(FindObjectsSortMode.None);
        if (allDoors.Length > 0)
        {
            cheat.gameObject.SetActive(true);
            Debug.Log("Weapon Cheat - There are doors, toggling");
        }
    }
}
