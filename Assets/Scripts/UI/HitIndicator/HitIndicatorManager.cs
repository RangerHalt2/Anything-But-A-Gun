using Unity.VisualScripting;
using UnityEngine;

public class HitIndicatorManager : MonoBehaviour
{
    [SerializeField] private RectTransform spawnPoint;
    [SerializeField] private GameObject hitIndicatorPrefab;

    private PlayerController playerController;
    private Health playerHealth;

    void OnDisable() => playerHealth.PlayerTookDamage -= SpawnIndicator;

    private void Start()
    {
        playerController = GameObject.FindAnyObjectByType<PlayerController>();
        playerHealth = playerController.GetComponent<Health>();
        playerHealth.PlayerTookDamage += SpawnIndicator;
    }

    private void SpawnIndicator(Health.DamageInfo info)
    {
        GameObject spawnedHit = Instantiate(hitIndicatorPrefab, spawnPoint.transform);
        HitIndicator indicator = spawnedHit.GetComponent<HitIndicator>();

        indicator.Initialize(info.source);
    }
}
