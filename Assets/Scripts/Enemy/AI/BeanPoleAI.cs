using UnityEngine;
using UnityEngine.AI;

public class BeanPoleAI : EnemyClass
{
    [Header("Donut Movement Settings")]
    public float minRange = 6f;
    public float maxRange = 8f;
    public float orbitWidthMultiplier = 1.5f;

    private float orbitDirection;
    private float orbitAngle;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = true;

        orbitDirection = Random.value > 0.5f ? 1f : -1f;

        if (detectionSprite != null)
            detectionSprite.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        transform.LookAt(player);

        agent.isStopped = false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float orbitRadius = (minRange + maxRange) / 2f;

        if (distanceToPlayer > maxRange)
        {
            Vector3 target = (transform.position - player.position).normalized * orbitRadius + player.position;
            agent.SetDestination(target);
        }
        else if (distanceToPlayer < minRange)
        {
            Vector3 target = (transform.position - player.position).normalized * orbitRadius + player.position;
            agent.SetDestination(target);
        }
        else
        {
            float circumference = 2f * Mathf.PI * orbitRadius;
            float anglePerSecond = agent.speed / circumference * 360f;

            orbitAngle += orbitDirection * anglePerSecond * Time.fixedDeltaTime;
            orbitAngle %= 360f;

            float rad = orbitAngle * Mathf.Deg2Rad;

            Vector3 orbitTarget = player.position + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius * orbitWidthMultiplier;

            agent.SetDestination(orbitTarget);
        }

        if (distanceToPlayer <= attackRange)
        {
            Attacking();
        }

        if (!hasShownDetectionSprite)
        {
            ShowDetectionSprite();
        }

        if (Random.value < 0.005f)
        {
            orbitDirection *= -1f;
        }
    }

    private void ShowDetectionSprite()
    {
        if (detectionSprite != null)
        {
            detectionSprite.SetActive(true);
            Invoke(nameof(HideDetectionSprite), detectionDisplayTime);
            hasShownDetectionSprite = true;
        }
    }

    private void HideDetectionSprite()
    {
        if (detectionSprite != null)
            detectionSprite.SetActive(false);
    }

    public override void Attacking()
    {
        if (!alreadyAttacked && HasLineOfSight())
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, maxRange);
        }
    }
}