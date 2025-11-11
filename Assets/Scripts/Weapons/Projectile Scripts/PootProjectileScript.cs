using UnityEngine;

public class PootProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject pootPrefab;

    private WeaponLevel weaponLevelRef;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnDestroy()
    {
            Vector3 collisionPoint = transform.position;
                GameObject pootGameObject = Instantiate(pootPrefab, collisionPoint, Quaternion.identity, null);
                PootCloudScript pootPuddleScript = pootGameObject.GetComponent<PootCloudScript>();
                pootPuddleScript.SetWeaponLevelReference(weaponLevelRef);
                Destroy(gameObject);
    }
}
