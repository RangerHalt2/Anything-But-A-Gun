using UnityEngine;
using System.Collections;

public class BossAbilities : MonoBehaviour
{
    [Header("General Ability Settings")]
    public float abilityInterval = 10f;

    [Header("Shockwave Settings")]
    public GameObject shockwavePrefab;
    public int shockwaveCount = 3;
    public float waveSize = 2f;
    public float timeBetweenWaves = 0.5f;
    public Vector3 initialScale = Vector3.one;

    [Header("Missile Settings")]
    public GameObject missilePrefab;
    public float missileSpacing = 2f;

    [Header("Big Shot Settings")]
    public GameObject bigShotPrefab;
    public float chargeTime = 2f;
    public float fireDelay = 1f;
    public float bigShotSpeed = 20f;
    public Transform shootPoint;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("BossAbilities: Player reference not assigned!");
            return;
        }

        StartCoroutine(AbilityLoop());
    }

    IEnumerator AbilityLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(abilityInterval);

            int randomAbility = Random.Range(0, 3);

            switch (randomAbility)
            {
                case 0:
                    yield return StartCoroutine(ShockwaveAttack());
                    break;

                case 1:
                    MissileStrike();
                    break;

                case 2:
                    yield return StartCoroutine(AbilityThree());
                    break;
            }
        }
    }

    IEnumerator ShockwaveAttack()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        direction.Normalize();

        Vector3 spawnPosition = transform.position;
        Vector3 currentScale = initialScale;

        float previousLength = currentScale.z;

        for (int i = 0; i < shockwaveCount; i++)
        {
            float newLength = previousLength * waveSize;

            float spacing = (previousLength / 2f) + (newLength / 2f);
            spawnPosition += direction * spacing;

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 5f, Vector3.down, out hit, 10f))
            {
                Vector3 wavePosition = hit.point;

                currentScale.x = newLength;
                currentScale.z = newLength;
                currentScale.y = newLength * 0.2f;

                wavePosition += Vector3.up * (currentScale.y / 2f);

                GameObject shockwave = Instantiate(shockwavePrefab, wavePosition, Quaternion.LookRotation(direction));

                shockwave.transform.localScale = currentScale;
            }

            previousLength = newLength;

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void MissileStrike()
    {
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-missileSpacing, 0, missileSpacing),
            new Vector3(0, 0, missileSpacing),
            new Vector3(missileSpacing, 0, missileSpacing),

            new Vector3(-missileSpacing / 2f, 0, 0),
            new Vector3(missileSpacing / 2f, 0, 0),

            new Vector3(-missileSpacing, 0, -missileSpacing),
            new Vector3(0, 0, -missileSpacing),
            new Vector3(missileSpacing, 0, -missileSpacing)
        };

        Vector3 centerPosition = player.position;

        foreach (Vector3 offset in offsets)
        {
            Vector3 spawnPos = centerPosition + offset;
            SpawnMissileOnGround(spawnPos);
        }

        Debug.Log("BossAbilities: Missile Strike triggered!");
    }

    void SpawnMissileOnGround(Vector3 position)
    {
        RaycastHit hit;

        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            Instantiate(missilePrefab, hit.point, Quaternion.identity);
        }
    }

    IEnumerator AbilityThree()
    {
        Debug.Log("BossAbilities: Charging Big Shot...");

        BossAI bossAI = GetComponent<BossAI>();
        if (bossAI != null)
        {
            bossAI.SetMovementEnabled(false);
        }

        yield return new WaitForSeconds(chargeTime);

        Vector3 lockedPlayerPosition = player.position;

        yield return new WaitForSeconds(fireDelay);

        Vector3 direction = (lockedPlayerPosition - shootPoint.position).normalized;

        GameObject shot = Instantiate(bigShotPrefab, shootPoint.position, Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f));

        Rigidbody rb = shot.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bigShotSpeed;
        }

        Debug.Log("BossAbilities: Big Shot Fired!");

        if (bossAI != null)
        {
            bossAI.SetMovementEnabled(true);
        }
    }
}