using UnityEngine;
using UnityEngine.AI;

public class NuggieBehaviorScript : MonoBehaviour, IWeaponLevel
{
    [SerializeField] private float nuggieTimer;

    [SerializeField] private CentralEnemyManager enemyManager;

    [Header("Target Layers and Target")]
    [SerializeField] private NavMeshAgent agent;
    private Transform target;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Set Walk Point")]
    private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange = 10f;

    [Header("Attack Cooldown")]
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private bool alreadyAttacked;

    [Header("Attack Seetings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float nuggieBaseDamage = 5;
    [SerializeField] private float growthRate = 1.15f;
    private float cummulativeDamage;

    [Header("Sight and Attack Range")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float attackRange = 10f;
    private bool targetInSightRange;
    private bool targetInAttackRange;

    private WeaponLevel currentWeaponLevel;

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
        if (currentWeaponLevel != null) UpdateLevelDamage();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyManager = CentralEnemyManager.Instance;
        
    }

    private void FixedUpdate() 
    {
        if (enemyManager.ActiveEnemyCount() > 0)
        {
            target = enemyManager.FindClosestEnemy(transform.position).transform; //The target the nuggie is chasing
        }
        else 
        {
            target = null;
        }
        nuggieTimer -= 1 * Time.deltaTime; //Lifetime of the nuggie

        if (nuggieTimer < 0)
        {
            Destroy(gameObject);
        }

        if (target != null)
        {
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
        else 
        { 
            Patrolling();
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
            MAULTHEM();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void MAULTHEM()
    {
        if (target != null)
        {
            target.gameObject.GetComponent<Health>().TakeDamage(nuggieBaseDamage, this.transform);
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 direction = (target.position - attackPoint.position).normalized;
        float distance = Vector3.Distance(attackPoint.position, target.position);
        
        if (Physics.Raycast(attackPoint.position, direction, out RaycastHit hit, distance))
        {
            return hit.transform.CompareTag("Enemy");
        }
        return false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //LB: Updates the weapon's damage for what damage it should do.
    public void UpdateLevelDamage()
    {
        cummulativeDamage = nuggieBaseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level);
    }
}
