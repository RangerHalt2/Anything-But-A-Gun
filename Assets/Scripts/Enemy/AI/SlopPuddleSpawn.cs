using UnityEngine;
using System.Collections;

public class SlopPuddleSpawn : MonoBehaviour
{
    [SerializeField] private GameObject puddlePrefab;

    IEnumerator Start()
    {
        Collider col = GetComponent<Collider>();
        col.enabled = false;

        yield return new WaitForSeconds(0.05f);

        col.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        Instantiate(puddlePrefab, hitPoint, Quaternion.identity);

        Destroy(gameObject);
    }
}