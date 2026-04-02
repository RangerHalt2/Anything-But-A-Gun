using System;
using UnityEngine;

public class PootProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject pootPrefab;

    private WeaponLevel weaponLevelRef;

    [HideInInspector] public Type promotion;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnDestroy() //Can be changed to OnDestroy, IF and ONLY IF every wall is registered as a Wall in the tags.
    {
        Vector3 collisionPoint = transform.position;
        GameObject pootGameObject = Instantiate(pootPrefab, collisionPoint, Quaternion.identity, null);
        PootCloudScript pootPuddleScript = pootGameObject.GetComponent<PootCloudScript>();
                
        if(promotion == typeof(P_Prolonged))
        {
            pootGameObject.AddComponent<P_Prolonged>();
        }

        pootPuddleScript.SetWeaponLevelReference(weaponLevelRef);
        Destroy(gameObject);
    }
}
