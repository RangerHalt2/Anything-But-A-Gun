using UnityEngine;
using System.Collections.Generic;

public class CameraWeaponScript : MonoBehaviour, IWeapon
{
    [SerializeField] public int level {get; set;}

    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private AmmoManager ammoManager;
    private float lastFired = Mathf.NegativeInfinity;

    [SerializeField] private GameObject enemyCopy; //The copy for the enemy

    private List<GameObject> enemiesInRange = new List<GameObject>();
    [SerializeField] private GameObject gunShot;

    public void Shoot() //Haha, because camera?
    {
        // If enough time has passed since the last round was fired
        if ((Time.timeSinceLevelLoad - lastFired) > fireRate)
        {
            // If there is an assigned ammo manager, and that ammo manager has at least one round of ammo loaded
            if (ammoManager != null && ammoManager.GetCurrentAmmo() > 0)
            {
                // Attempt to fire the weapon
                ammoManager.Fire();
                // If the weapon is not reloading
                if (!ammoManager.IsReloading())
                {
                        SnapCamera();
                    // Update lastFired
                    lastFired = Time.timeSinceLevelLoad;
                }
            }
        }
    }

    public void Reload()
    {
        // If the shooter has at least one round of reserve ammo or is set to have infinite ammo
        if (ammoManager.GetReserveAmmo() > 0 || ammoManager.GetReserveAmmo() == -1)
        {
            // Reload the shooter
            ammoManager.ReloadWeapon();
        }
    }

    //These next 2 just keep track of the enemy's presence in the trigger range

    private void OnTriggerEnter(Collider _other) 
    {
        if (_other.gameObject.CompareTag("Body")) 
        {
            enemiesInRange.Add(_other.gameObject);
            Debug.Log("Found enemy!!");
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.CompareTag("Body"))
        {
            enemiesInRange.Remove(_other.gameObject);
        }
    }

    private void SnapCamera()
    {
        foreach (GameObject enemy in enemiesInRange) //Create a snapshot of every enemy in range
        {
            GameObject target = enemy.transform.parent.gameObject;
            GameObject snapshot = Instantiate(enemyCopy, target.transform.position, target.transform.rotation, null);
            snapshot.SetActive(false);
            snapshot.GetComponent<SnapshotBehaviorScript>().copyOf = target;
            snapshot.SetActive(true);
        }
    }
}
