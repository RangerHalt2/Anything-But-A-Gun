using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    private WeaponHandler wh;
    private AmmoManager am;

    public bool canInteract { get; set; } = true;

    public int magazineGain = 2; //Number of full magazines that the weapon gets upon pickup.

    public void Interact() 
    {
        wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        am = wh.currentWeapon.GetComponent<AmmoManager>();

        if (am != null) 
        {
            int ammoGain = Mathf.RoundToInt(am.maxAmmo / 2);
            Debug.Log("Ammo Box found weapon to gain ammo");
            am.reserveAmmo += ammoGain;
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
