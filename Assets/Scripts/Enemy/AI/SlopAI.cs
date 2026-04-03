using UnityEngine;
using UnityEngine.AI;

public class SlopAI : EnemyClass
{
    [Header("Slop Mortar Settings")]
    public float minRange = 15f;
    public float mortarRange = 40f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = true;

        if (detectionSprite != null)
            detectionSprite.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        transform.LookAt(player);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < minRange)
        {
            Vector3 retreatDir = (transform.position - player.position).normalized;
            Vector3 retreatTarget = player.position + retreatDir * minRange;

            agent.isStopped = false;
            agent.SetDestination(retreatTarget);
        }

        else
        {
            agent.isStopped = true;

            if (distanceToPlayer <= mortarRange)
            {
                Attacking();
            }
        }

        if (!hasShownDetectionSprite)
        {
            ShowDetectionSprite();
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
        if (!alreadyAttacked)
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mortarRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }
}