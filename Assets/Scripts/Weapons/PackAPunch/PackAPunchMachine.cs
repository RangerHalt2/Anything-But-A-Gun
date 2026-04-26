using UnityEngine;

public class PackAPunchMachine : MonoBehaviour, IInteractable
{
    public int numOfUses = 1; //Public because this is going to be changed later and fuck getters amirite or amirite
    private int counter = 0;

    [SerializeField] private Transform placeWeaponLocation;
    [SerializeField] private GameObject[] visualSparks;

    [SerializeField] private string[] denials = new string[1];
    public string[] denyText { get { return denials; } set { denials = value; } }

    [SerializeField] private int price;

    private WeaponHandler wh;
    private WeaponClass wc;

    public bool canInteract { get; set; } = true;
    [Tooltip("Allows the promotion machine to ignore the player's currently unlocked achievements and spawn even if conditions are not met.")]
    [SerializeField] private bool forceSpawn = true;

    [SerializeField] GameObject sfx;

    // RL: Added check for Achievement Manager to see if Promotion Machines should be enable based on whether or not the player has the achievement beat_first_level AKA Nine-To-Five
    public void Start()
    {
        if (AchievementManager.Instance != null)
        {
            // If the player has not obtained the achievement Nine-To-Five (ID: beat_first_level) and the promotion is not set to be force spawned
            if (!AchievementManager.Instance.CheckAchivementStatus("beat_first_level") && !forceSpawn)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Interact()
    {
        Debug.Log("Pack-A-Punch Machine Begin");
        wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        wc = wh.currentWeapon.GetComponent<WeaponClass>();
        Debug.Log(wc);
        Debug.Log(wh);

        if (EconomyManager.Instance == null || EconomyManager.Instance.PTOAmount < price)
        {
            Debug.Log("PACK A PUNCH MACINE - the player does not have enough money or the EconomyManager is null");
            GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>().DenyInteract(denyText[0]);
            return;
        }
        else if (wh.currentWeapon.GetComponent<StaplerScript>())
        {
            if (wc == wh.currentWeapon.GetComponent<StaplerScript>())
            {
                Debug.Log("Pack-a-Punch - Denied");
                GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>().DenyInteract(denyText[1]);
                return;
            }
        }

        if (wc != null && wc.GetPackAPunchIndex() == -1 && counter < numOfUses)
        {
            Debug.Log("Pack-A-Punch Machine determined it can be added");
            int selectedPunch = Random.Range(0, wc.GetPackAPunchLength());
            wc.SetPackAPunchIndex(selectedPunch);
            bool worked = wc.AddPackAPunch();
            if (worked)
            {
                if(sfx != null)
                    Instantiate(sfx, transform.position, transform.rotation, null);
                counter++;
                GameEvent.OnWeaponModified?.Invoke();
                EconomyManager.Instance.PTOAmount -= price;
            }
            if (counter == numOfUses)
            {
                canInteract = false;
                foreach (GameObject obj in visualSparks)
                {
                    obj.SetActive(false);
                }
            }
        }
        else if (counter >= numOfUses)
        {
            GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>().DenyInteract(denyText[2]);
            return;
        }
        else if (wc.GetPackAPunchIndex() != -1)
        {
            GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerController>().DenyInteract(denyText[3]);
            return;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            WeaponClass wc = collision.gameObject.GetComponent<WeaponClass>();
            if (wc != null && placeWeaponLocation != null)
            {
                wc.gameObject.transform.position = placeWeaponLocation.transform.position;
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
            WeaponClass wc = other.gameObject.GetComponent<WeaponClass>();
            if (wc != null && placeWeaponLocation != null)
            {
                wc.gameObject.transform.position = placeWeaponLocation.transform.position;
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
