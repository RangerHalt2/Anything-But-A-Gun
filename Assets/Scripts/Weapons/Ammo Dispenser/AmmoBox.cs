using System;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    private WeaponHandler wh;
    private AmmoManager am;
    private WeaponClass wc;
    private Type promotionType;

    public bool canInteract { get; set; } = true;

    public int magazineGain = 2; //Number of full magazines that the weapon gets upon pickup.

    public void Interact() 
    {
        wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        am = wh.currentWeapon.GetComponent<AmmoManager>();

        wc = wh.currentWeapon.GetComponent<WeaponClass>();
        if(wc != null && wc.hasPackAPunch)
        {
            promotionType = wc.currPackAPunchComponent.GetType();
        }

        if (am != null) 
        {
            int ammoGain = Mathf.RoundToInt(am.maxAmmo / 2);
            Debug.Log("Ammo Box found weapon to gain ammo");
            if(promotionType != null && promotionType != typeof(P_BottomlessMag))
                am.reserveAmmo += ammoGain;
            if (promotionType != null && promotionType == typeof(P_BottomlessMag))
                am.SetCurrentAmmo(am.GetCurrentAmmo() + ammoGain);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            pc.CheckInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            pc.CheckInteract();
            pc.LeftInteract();
        }
    }
}
