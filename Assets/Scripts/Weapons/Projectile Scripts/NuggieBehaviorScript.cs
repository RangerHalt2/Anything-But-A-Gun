using UnityEngine;
using UnityEngine.AI;

public class NuggieBehaviorScript : MonoBehaviour
{
    [SerializeField] private float nuggieTimer;

    [SerializeField] private Transform player;
    [SerializeField] private CentralEnemyManager enemyManager;

    [Header("Target Layers and Target")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Set Walk Point")]
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool walkPointSet;
    [SerializeField] private float walkPointRange = 10f;

    [Header("Attack Cooldown")]
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private bool alreadyAttacked;

    [Header("Bullet Seetings")]
    [SerializeField] private GameObject nuggieBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 10f;

    [Header("Sight and Attack Range")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] public float attackRange = 10f;
    [SerializeField] private bool targetInSightRange;
    [SerializeField] public bool targetInAttackRange;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyManager = CentralEnemyManager.Instance;
        
    }

    private void FixedUpdate()
    {
        target = enemyManager.FindClosestEnemy(transform.position).transform;

        nuggieTimer -= nuggieTimer * Time.deltaTime;

        if (nuggieTimer < 0)
        {
            Destroy(gameObject);
        }

        transform.LookAt(target);

        targetInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsTarget);
        targetInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsTarget);

        if (!targetInSightRange && !targetInAttackRange)
        {
            Patrolling();
        }
        if (targetInSightRange && !targetInAttackRange)
        {
            Chase();
        }
        if (targetInSightRange && targetInAttackRange)
        {
            Attacking();
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && IsWalkPointReachable(walkPoint))
        {
            walkPointSet = true;
        }
    }

    private bool IsWalkPointReachable(Vector3 targetPoint)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPoint, path);

        return path.status == NavMeshPathStatus.PathComplete;
    }

    private void Patrolling()
    {
        agent.isStopped = false;

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);
        agent.isStopped = true;

        if (!alreadyAttacked && HasLineOfSight())
        {
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Shoot()
    {
        if (nuggieBulletPrefab != null && firePoint != null)
        {
            Debug.Log("SHOOOT HIM");

            Vector3 direction = (target.position - firePoint.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Quaternion finalRotation = lookRotation * Quaternion.Euler(0f, 0f, 0f);

            GameObject bullet = Instantiate(nuggieBulletPrefab, firePoint.position, finalRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 direction = (target.position - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, target.position);

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, distance))
        {
            return hit.transform.CompareTag("Enemy");
        }
        return false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
