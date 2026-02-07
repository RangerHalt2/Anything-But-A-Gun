using UnityEngine;

public class LavaProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject puddlePrefab;

    private WeaponLevel weaponLevelRef;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnCollisionEnter(Collision _other)
    {
        Vector3 collisionPoint = transform.position;
        ContactPoint contactPoint = _other.GetContact(0);
        GameObject puddleGameObject = Instantiate(puddlePrefab, contactPoint.point + new Vector3(0f, 1f, 0f), Quaternion.identity, null);
        LavaPuddleScript lavaPuddleScript = puddleGameObject.GetComponent<LavaPuddleScript>();
        if (gameObject.GetComponent<P_Heavy>() != null)
        {
            lavaPuddleScript.externalDamageMod = gameObject.GetComponent<P_Heavy>().extraDamage;
        }
        lavaPuddleScript.SetWeaponLevelReference(weaponLevelRef);
        Destroy(gameObject);

    }
}
