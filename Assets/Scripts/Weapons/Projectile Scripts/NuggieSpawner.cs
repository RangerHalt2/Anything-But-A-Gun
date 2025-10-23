using UnityEngine;

public class NuggieSpawner : MonoBehaviour
{ 
    [SerializeField] private GameObject nuggiePrefab;

    void OnTriggerEnter(Collider _other)
    {
            GameObject nuggieGameObject = Instantiate(nuggiePrefab, transform.position, transform.rotation, null);
            Destroy(gameObject);
    }
}
