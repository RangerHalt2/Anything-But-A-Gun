using UnityEngine;

public class LavaProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject puddlePrefab;

    void OnTriggerEnter(Collider _other)
    {
            Vector3 collisionPoint = transform.position;
            RaycastHit hit;
            if (Physics.Raycast(collisionPoint, Vector3.down, out hit))
            {
                GameObject puddleGameObject = Instantiate(puddlePrefab, hit.point, Quaternion.identity, null);
                Destroy(gameObject);
            }
        
    }
}
