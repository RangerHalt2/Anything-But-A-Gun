using UnityEngine;

public class NuggieSpawner : MonoBehaviour
{ 
    [SerializeField] private GameObject nuggiePrefab;

    private WeaponLevel weaponLevelRef;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        weaponLevelRef = weaponLevel;
    }

    void OnDestroy()
    {
            GameObject nuggieGameObject = Instantiate(nuggiePrefab, transform.position, transform.rotation, null);
            NuggieBehaviorScript nbs = nuggieGameObject.GetComponent<NuggieBehaviorScript>();
            nbs.SetWeaponLevelReference(weaponLevelRef);
            Destroy(gameObject);
    }
}
