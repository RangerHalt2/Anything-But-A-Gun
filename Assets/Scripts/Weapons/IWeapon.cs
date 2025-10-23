using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public interface IWeapon
{
    // The level of the weapon
    int level { get; set; }

    void Shoot();
    void Reload();
}
