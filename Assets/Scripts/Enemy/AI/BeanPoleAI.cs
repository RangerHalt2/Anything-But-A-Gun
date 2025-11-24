using UnityEngine;
using UnityEngine.AI;

public class BeanPoleAI : EnemyClass
{
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
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attacking();
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
        {
            detectionSprite.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}