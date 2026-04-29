using Unity.VisualScripting;
using UnityEngine;

public class HitIndicatorManager : MonoBehaviour
{
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private GameObject hitIndicatorPrefab;

    private PlayerController playerController;
    public Health playerHealth;

    private void Start()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerHealth = playerController.GetComponent<Health>();
            if (playerHealth != null)
            {
                Debug.Log("HIT INDICATOR MANAGER - Instance ID: " + playerHealth.GetInstanceID());
                playerHealth.PlayerTookDamage += SpawnIndicator;
                Debug.Log("HIT INDICATOR MANAGER - Player Controller, Health, and Listeners are complete");
                Debug.Log("HIT INDICATOR MANAGER - Subscribed: " + playerHealth.HasDamageListeners());
            }
        }

    }

    //public void Confirm

    private void Update()
    {
        if (playerController == null)
        {
            playerController = GameObject.FindAnyObjectByType<PlayerController>();
        }

        if (playerHealth == null)
        {
            playerHealth = playerController.GetComponent<Health>();
            playerHealth.PlayerTookDamage += SpawnIndicator;
        }
    }

    public void AssignTookDamageEvent()
    {
        playerHealth.PlayerTookDamage += SpawnIndicator;
    }

    private void SpawnIndicator(Health.DamageInfo info)
    {
        if (spawnPoint == null)
        {
            HitIndicatorSpawnPoint spawnIndicatorPoint = GameObject.FindAnyObjectByType<HitIndicatorSpawnPoint>();
            if (spawnIndicatorPoint != null)
            {
                spawnPoint = spawnIndicatorPoint.gameObject.GetComponent<RectTransform>();
            }
        }

        if (spawnPoint == null)
            {
                Debug.LogError("HIT INDICATOR MANAGER - SPAWN POINT IS STILL NULL");
                return;
            }
        GameObject spawnedHit = Instantiate(hitIndicatorPrefab, spawnPoint.transform);
        spawnedHit.transform.localScale = Vector3.one;
        spawnedHit.transform.localPosition = Vector3.zero;
        HitIndicator indicator = spawnedHit.GetComponent<HitIndicator>();

        indicator.Initialize(info.source);
        Debug.Log("HIT INDICATOR MANAGER - Player has been hit, spawned the indicator with source: " + info.source);
    }
}
