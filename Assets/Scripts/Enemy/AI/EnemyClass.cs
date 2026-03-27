using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    [Header("Player Detection")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;

    [Header("Enemy Projectile")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float bulletForce = 10f;

    [Header("Patrol")]
    public float patrolRange = 10f;
    public float patrolWaitTime = 2f;
    public float patrolTimer;

    [Header("Sight and Attack Range")]
    public float sightRange = 10f;
    public float attackRange = 10f;
    public float timeBetweenAttacks = 2.5f;
    public bool alreadyAttacked;
    public bool playerInSightRange;
    public bool playerInAttackRange;

    [Header("Detection Indicator")]
    public GameObject detectionSprite;
    public float detectionDisplayTime = 1f;
    public bool hasShownDetectionSprite = false;

    public event Action<EnemyClass> OnEnemyDeath;

    public EnemyType enemyType;

    public float height = 2f;

    [SerializeField] protected Animator animator;

    public enum EnemyType
    {
        Common,
        Elite,
        Boss
    }


    public Vector3 GetRandomNavMeshPosition(Vector3 center, float range)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range;
        randomDirection += center;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, range, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return center;
    }

    public virtual void Patrolling()
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

    public virtual void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    public virtual void Attacking()
    {
        agent.SetDestination(player.position);
        agent.isStopped = false;

        if (!alreadyAttacked && HasLineOfSight())
        {
            Debug.Log("ENEMY CLASS - Enemy has line of sight and is attacking!");
            Shoot();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    public virtual void Shoot()
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
            if(bullet.TryGetComponent(out Projectile projectile))
            {
                projectile.enemyPosition = this.transform;
            }
        }
    }

    public virtual bool HasLineOfSight()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, player.position);

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, distance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    public virtual void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void Die()
    {
       OnEnemyDeath?.Invoke(this);
    }

}
