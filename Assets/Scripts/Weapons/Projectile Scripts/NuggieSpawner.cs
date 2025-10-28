using UnityEngine;
using UnityEngine.AI;

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
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position, out hit, 5.0f, UnityEngine.AI.NavMesh.AllAreas);

        GameObject nuggieGameObject = Instantiate(nuggiePrefab, hit.position, transform.rotation, null);
        
        NuggieBehaviorScript nbs = nuggieGameObject.GetComponent<NuggieBehaviorScript>();
        nbs.SetWeaponLevelReference(weaponLevelRef);
    }
}
