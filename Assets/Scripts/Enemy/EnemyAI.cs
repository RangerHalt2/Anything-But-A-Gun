using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target Layers and Player")]
    [SerializeField]private NavMeshAgent agent;
    [SerializeField]private Transform player;
    [SerializeField]private LayerMask whatIsGround;
    [SerializeField]private LayerMask whatIsPlayer;

    [Header("Set Walk Point")]
    [SerializeField]private Vector3 walkPoint;
    [SerializeField]private bool walkPointSet;
    [SerializeField]private float walkPointRange = 10;

    [Header("Attack Cooldown")]
    [SerializeField]private float timeBetweenAttacks = 2.5f;
    [SerializeField]private bool alreadyAttacked;

    [Header("Bullet Seetings")]
    [SerializeField]private GameObject enemyBulletPrefab;
    [SerializeField]private Transform firePoint;
    [SerializeField]private float bulletForce = 10f;

    [Header("Sight and Attack Range")]
    [SerializeField]private float sightRange = 10;
    [SerializeField]public float attackRange = 10;
    [SerializeField]private bool playerInSightRange;
    [SerializeField]public bool playerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange)
        {
            Patrolling();
        }
        if(playerInSightRange && !playerInAttackRange)
        {
            Chase();
        }
        if(playerInSightRange && playerInAttackRange)
        {
            Attacking();
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround) && IsWalkPointReachable(walkPoint))
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
        if(!walkPointSet) 
        {
            SearchWalkPoint();
        }
        if(walkPointSet) 
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
        
    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        transform.LookAt(player);

        if(!alreadyAttacked)
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

            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Quaternion finalRotation = lookRotation * Quaternion.Euler(0f, 0f, 0f);

            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, finalRotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
