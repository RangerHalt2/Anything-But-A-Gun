using UnityEngine;
using UnityEngine.AI;

public class BrickAI : MonoBehaviour
{
    [Header("Target Layers and Player")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolRange = 10f;
    [SerializeField] private float patrolWaitTime = 2f;
    private float patrolTimer;

    [Header("Attack Cooldown")]
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private bool alreadyAttacked;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 10f;

    [Header("Sight and Attack Range")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] public float attackRange = 10f;
    [SerializeField] private bool playerInSightRange;
    [SerializeField] public bool playerInAttackRange;

    [Header("Investigation Settings")]
    private Vector3 lastKnownPlayerPosition;
    private bool isInvestigating;
    private float investigationWaitTime = 3f;
    private float investigationTimer;

    [Header("Detection Indicator")]
    [SerializeField] private GameObject detectionSprite;
    [SerializeField] private float detectionDisplayTime = 1f;
    private bool hasShownDetectionSprite = false;

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
            }
            else
            {
                Attacking();
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

    private void Patrolling()
    {
        agent.isStopped = false;
        patrolTimer += Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            if (patrolTimer >= patrolWaitTime)
            {
                Vector3 newDestination = GetRandomNavMeshPosition(transform.position, patrolRange);
                agent.SetDestination(newDestination);
                patrolTimer = 0f;
            }
        }
    }

    private Vector3 GetRandomNavMeshPosition(Vector3 center, float range)
    {
        Vector3 randomDirection = Random.insideUnitSphere * range;
        randomDirection += center;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, range, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return center;
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        agent.SetDestination(player.position);
        agent.isStopped = false;

        if (!alreadyAttacked && HasLineOfSight())
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Shoot()
    {
        if (enemyBulletPrefab != null && firePoint != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, rotation);

            if (bullet.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, player.position);

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, distance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

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