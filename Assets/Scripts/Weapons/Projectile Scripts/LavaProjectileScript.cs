using UnityEngine;

public class LavaProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject puddlePrefab;

    private WeaponLevel weaponLevelRef;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnTriggerEnter(Collider _other)
    {
            Vector3 collisionPoint = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(collisionPoint, Vector3.down, out hit))
            {
                GameObject puddleGameObject = Instantiate(puddlePrefab, hit.point, Quaternion.identity, null);
                LavaPuddleScript lavaPuddleScript = puddleGameObject.GetComponent<LavaPuddleScript>();
                lavaPuddleScript.SetWeaponLevelReference(weaponLevelRef);
                Destroy(gameObject);
            }
        
    }
}
