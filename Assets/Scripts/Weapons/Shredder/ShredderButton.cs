using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShredderButton: MonoBehaviour, IInteractable
{
    public int numOfUses = 1; //Public because this is going to be changed later and fuck getters amirite or amirite
    private int counter = 0;

    public bool canInteract { get; set; } = true;
    private PlayerController pc;
    [SerializeField] private GameObject blastVFX;

    HashSet<GameObject> uniqueObjects = new HashSet<GameObject>();

    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private BoxCollider boxTrigger;

    private void Start() 
    {
        pc = GameObject.FindAnyObjectByType<PlayerController>();
    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;

        Vector3 center = boxTrigger.transform.position;
        Vector3 size = boxTrigger.size;

        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, size);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    public void Interact()
    {
        if (counter < numOfUses)
        {
            counter++;
            StartCoroutine(Shredder());
        }
    }

    private IEnumerator Shredder() 
    {
        uniqueObjects = new HashSet<GameObject>();
        Debug.Log("ShredderButton attempted");
        Vector3 center = boxTrigger.transform.position;
        Vector3 halfExtents = boxTrigger.size * 0.5f;
        Quaternion orientation = transform.rotation;
        Collider[] hits = Physics.OverlapBox(center, halfExtents, orientation, interactableLayer);

        bool blast = false;

        foreach (Collider weapon in hits)
        {
            WeaponClass check = weapon.gameObject.GetComponent<WeaponClass>();
            if (check != null)
            {
                if (blastVFX != null && blast == false)
                {
                    blast = true;
                    Instantiate(blastVFX, center, orientation);
                    yield return new WaitForSeconds(3);
                }

                uniqueObjects.Add(check.gameObject);
            }

        }

        foreach (GameObject obj in uniqueObjects)
        {
            Debug.Log("ShreddedButton tried to delete " + obj);

            pc.GetComponent<EconomyManager>().PTOAmount += obj.GetComponent<WeaponClass>().PTOAmount;
            Destroy(obj);
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
