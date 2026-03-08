using Unity.VisualScripting;
using UnityEngine;

public class HitIndicatorManager : MonoBehaviour
{
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private GameObject hitIndicatorPrefab;

    private PlayerController playerController;
    private Health playerHealth;

    private void Start()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
        playerHealth = playerController.GetComponent<Health>();
        Debug.Log("HIT INDICATOR MANAGER - Instance ID: " + playerHealth.GetInstanceID());
        playerHealth.PlayerTookDamage += SpawnIndicator;
        Debug.Log("HIT INDICATOR MANAGER - Player Controller, Health, and Listeners are complete");
        Debug.Log("HIT INDICATOR MANAGER - Subscribed: " + playerHealth.HasDamageListeners());
    }

    //public void Confirm

    private void SpawnIndicator(Health.DamageInfo info)
    {
        GameObject spawnedHit = Instantiate(hitIndicatorPrefab, spawnPoint.transform);
        spawnedHit.transform.localScale = Vector3.one;
        HitIndicator indicator = spawnedHit.GetComponent<HitIndicator>();

        indicator.Initialize(info.source);
        Debug.Log("HIT INDICATOR MANAGER - Player has been hit, spawned the indicator with source: " + info.source);
    }
}
