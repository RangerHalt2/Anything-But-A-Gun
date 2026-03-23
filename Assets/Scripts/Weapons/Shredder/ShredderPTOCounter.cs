using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShredderPTOCounter : MonoBehaviour
{
    public bool canInteract { get; set; } = true;

    public List<GameObject> weaponsToDestroy = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            WeaponClass wc = other.gameObject.GetComponent<WeaponClass>();
            if (wc != null && !wc.gameObject.GetComponent<WeaponCollectScript>().collected)
            {
                weaponsToDestroy.Add(wc.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            WeaponClass wc = other.gameObject.GetComponent<WeaponClass>();
            weaponsToDestroy.Remove(wc.gameObject);
        }
    }

}
