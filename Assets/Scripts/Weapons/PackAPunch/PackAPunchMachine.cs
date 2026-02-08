using UnityEngine;

public class PackAPunchMachine : MonoBehaviour, IInteractable
{
    public int numOfUses = 1; //Public because this is going to be changed later and fuck getters amirite or amirite
    private int counter = 0;

    [SerializeField] private Transform placeWeaponLocation;

    private WeaponHandler wh;
    private WeaponClass wc;

    public bool canInteract { get; set; } = true;

    public void Interact()
    {
        Debug.Log("Pack-A-Punch Machine Begin");
        wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        wc = wh.currentWeapon.GetComponent<WeaponClass>();

        if(wc != null && wc.GetPackAPunchIndex() == -1 && counter < numOfUses)
        {
            Debug.Log("Pack-A-Punch Machine determined it can be added");
            int selectedPunch = Random.Range(0, wc.GetPackAPunchLength());
            wc.SetPackAPunchIndex(selectedPunch);
            bool worked = wc.AddPackAPunch();
            if (worked)
            {
                counter++;
            }
            if (counter == numOfUses)
            {
                canInteract = false;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            WeaponClass wc = other.gameObject.GetComponent<WeaponClass>();
            if (wc != null && placeWeaponLocation != null)
            {
                wc.gameObject.transform.position = placeWeaponLocation.transform.position;
            }
        }
    }

}
