using UnityEngine;
using UnityEngine.AI;

public class BeanPoleAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private bool alreadyAttacked;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 10f;

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