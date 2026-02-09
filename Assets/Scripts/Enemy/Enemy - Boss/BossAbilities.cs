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

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("BossAbilities: Player reference not assigned in the inspector!");
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
                    AbilityTwo();
                    break;
                case 2:
                    AbilityThree();
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

        for (int i = 0; i < shockwaveCount + 1; i++)
        {
            spawnPosition += direction * (1.5f * i);

            if (i == 0)
            {
                currentScale.x *= waveSize;
                currentScale.z *= waveSize;
                yield return new WaitForSeconds(timeBetweenWaves);
                continue;
            }

            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 5f, Vector3.down, out hit, 10f))
            {
                Vector3 wavePosition = hit.point;
                GameObject shockwave = Instantiate(shockwavePrefab, wavePosition, Quaternion.LookRotation(direction));

                shockwave.transform.localScale = currentScale;
            }
            else
            {
                Debug.LogWarning("BossAbilities: No ground found under spawn position!");
            }

            currentScale.x *= waveSize;
            currentScale.z *= waveSize;

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void AbilityTwo()
    {
        Debug.Log("BossAbilities: Ability Two triggered");
    }

    void AbilityThree()
    {
        Debug.Log("BossAbilities: Ability Three triggered");
    }
}