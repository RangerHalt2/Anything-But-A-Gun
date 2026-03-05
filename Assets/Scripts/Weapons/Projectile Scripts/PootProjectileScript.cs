using UnityEngine;

public class PootProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject pootPrefab;

    private WeaponLevel weaponLevelRef;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnTriggerEnter() //Can be changed to OnDestroy, IF and ONLY IF every wall is registered as a Wall in the tags.
    {
            Vector3 collisionPoint = transform.position;
                GameObject pootGameObject = Instantiate(pootPrefab, collisionPoint, Quaternion.identity, null);
                PootCloudScript pootPuddleScript = pootGameObject.GetComponent<PootCloudScript>();
                pootPuddleScript.SetWeaponLevelReference(weaponLevelRef);
                Destroy(gameObject);
    }
}
