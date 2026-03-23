using UnityEngine;

public class AmmoDispenser : MonoBehaviour, IInteractable
{
    public bool canInteract { get; set; } = true;

    public int numOfUses = 10000000; //Public because this is going to be changed later and fuck getters amirite or amirite
    private int counter = 0;
    public int price; //How much an ammo box costs 

    [SerializeField] private GameObject ammoBox; 

    [SerializeField] private Transform placeAmmoLocation;

    private EconomyManager ecoMan;

    private void Start() 
    {
        ecoMan = FindFirstObjectByType<EconomyManager>();
    }

    public void Interact()
    {
        if (ecoMan.SpendPTO(price) && counter < numOfUses)
        {
            Debug.Log("Ammo Dispenser Spawned Ammo");
            Instantiate(ammoBox, placeAmmoLocation.position, Quaternion.identity);
            counter++;
            
            if (counter >= numOfUses)
            {
                canInteract = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            AmmoBox ab = collision.gameObject.GetComponent<AmmoBox>();
            if (ab != null && placeAmmoLocation != null)
            {
                ab.gameObject.transform.position = placeAmmoLocation.transform.position;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            pc.CheckInteract();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            AmmoBox ab = other.gameObject.GetComponent<AmmoBox>();
            if (ab != null && placeAmmoLocation != null)
            {
                ab.gameObject.transform.position = placeAmmoLocation.transform.position;
            }
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

