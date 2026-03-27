using UnityEngine;
using UnityEngine.AI;

public class BrickAI : EnemyClass
{

    [Header("Investigation Settings")]
    private Vector3 lastKnownPlayerPosition;
    private bool isInvestigating;
    private float investigationWaitTime = 3f;
    private float investigationTimer;



    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = true;

        animator = GetComponent<Animator>();

        if (detectionSprite != null)
            detectionSprite.SetActive(false);
    }

    private void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        transform.LookAt(player);

        if (playerInSightRange)
        {
            lastKnownPlayerPosition = player.position;
            isInvestigating = false;

            if (!hasShownDetectionSprite)
            {
                ShowDetectionSprite();
            }

            if (!playerInAttackRange)
            {
                Chase();
                if (animator != null)
                    animator.SetBool("isPunching", false);
            }
            else
            {
                Attacking();
                if (animator != null)
                    animator.SetBool("isPunching", true);
            }
        }
        else if (!playerInSightRange && lastKnownPlayerPosition != Vector3.zero && !isInvestigating)
        {
            StartInvestigation();
        }
        else if (isInvestigating)
        {
            Investigate();
        }
        else
        {
            Patrolling();
        }

        if (!playerInSightRange && hasShownDetectionSprite)
        {
            hasShownDetectionSprite = false;
        }
    }

    //TO DO: Manage an "Alerted" variable in the patrol functions and chasing functions
    //TO DO: Check if line of sight, call the ShowDetectionSprite(), Call the Chase() function
    //public void AlertEnemy()

    private void StartInvestigation()
    {
        isInvestigating = true;
        investigationTimer = 0f;
        agent.isStopped = false;
        agent.SetDestination(lastKnownPlayerPosition);
    }

    private void Investigate()
    {
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            investigationTimer += Time.deltaTime;

            if (investigationTimer >= investigationWaitTime)
            {
                isInvestigating = false;
                lastKnownPlayerPosition = Vector3.zero;
            }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}