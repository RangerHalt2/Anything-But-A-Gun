using UnityEngine;
using UnityEngine.AI;

public class BossAI : EnemyClass
{
    private bool canMove = true;

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

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (canMove && agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else if (agent != null)
        {
            agent.isStopped = true;
        }

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

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (agent != null)
            agent.isStopped = !enabled;
    }
}