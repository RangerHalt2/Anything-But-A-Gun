using UnityEngine;

public class EnemyCamera : MonoBehaviour
{
    [Header("Player and Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private bool alreadyAttacked = false;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletForce = 15f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        FacePlayer();


        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            Attacking();
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
        }
    }

    private void Attacking()
    {
        //Debug.Log("TURRET - Attacking Function");
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
            GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * bulletForce, ForceMode.Impulse);
            }
        }
    }

    private bool HasLineOfSight()
    {
        //Debug.Log("TURRET - Entered Line of Sight");
        Vector3 direction = (player.position - firePoint.position).normalized;
        float distance = Vector3.Distance(firePoint.position, player.position);

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, distance, whatIsPlayer))
        {
            //Debug.Log("TURRET - bool: " + hit.transform.CompareTag("Player"));
            return hit.transform.CompareTag("Player");
        }

        return false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}