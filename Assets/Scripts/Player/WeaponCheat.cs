using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCheat : MonoBehaviour
{

    private Toggle self;


    private void Start()
    {
        self = GetComponent<Toggle>();
        self.onValueChanged.AddListener(ToggledEvent);
    }

    private void OnEnable()
    {
        DoorIdentifier[] allDoors = GameObject.FindObjectsByType<DoorIdentifier>(FindObjectsSortMode.None);
        if (allDoors.Length <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("Weapon Cheat - There are no doors, untoggling");
        }
    }

    private void ToggledEvent(bool isOn)
    {
        DoorIdentifier[] allDoors = GameObject.FindObjectsByType<DoorIdentifier>(FindObjectsSortMode.None);

        foreach (DoorIdentifier door in allDoors)
        {
            door.transform.localRotation = isOn
            ? Quaternion.Euler(0f, 150f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);
        }

    }

}
