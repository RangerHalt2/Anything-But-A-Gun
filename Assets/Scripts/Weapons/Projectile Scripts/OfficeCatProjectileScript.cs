using UnityEngine;

public class OfficeCatProjectileScript : MonoBehaviour
{
    private bool blackholed = false;
    private WeaponLevel weaponLevelRef;
    [SerializeField] private GameObject blackholePrefab;
    private BlackholeScript blackholeScript;

    void Update() 
    {

    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnTriggerEnter(Collider _other)
    {
        if (!blackholed)
        {
            blackholed = true;
            Vector3 collisionPoint = transform.position;
            GameObject blackholeGameObject = Instantiate(blackholePrefab, collisionPoint, Quaternion.identity, null);
            blackholeScript = blackholeGameObject.GetComponent<BlackholeScript>();
            blackholeScript.SetWeaponLevelReference(weaponLevelRef);
            //blackholeGameObject.SetActive(false);
        }
    }
}
