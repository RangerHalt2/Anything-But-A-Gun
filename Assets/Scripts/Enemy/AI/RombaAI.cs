using UnityEngine;
using UnityEngine.AI;

public class RombaAI : MonoBehaviour
{
    [Header("Target Layers and Player")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("NavMesh Patrol Settings")]
    [SerializeField] private float patrolDistance = 10f;
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float navEdgeCheckDistance = 2f;

    private Vector3 moveDirection;
    private float patrolTimer;

    [Header("Sight and Attack Range")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] public float attackRange = 2f;
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

    [Header("Sound and Visual Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private GameObject explosionPrefab;
    private bool soundPlayed = false;
    private bool explosionPlayed = false;

    private MeshRenderer meshRenderer;
    private Collider col;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = true;

        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();

        if (detectionSprite != null)
            detectionSprite.SetActive(false);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        PickRandomDirection();
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
                ShowDetectionSprite();

            if (!playerInAttackRange)
                Chase();
            else
                Attacking();
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
            NavMeshAwarePatrolling();
        }

        if (!playerInSightRange && hasShownDetectionSprite)
            hasShownDetectionSprite = false;
    }

    private void NavMeshAwarePatrolling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        Vector3 targetPoint = transform.position + moveDirection * patrolDistance;
        agent.SetDestination(targetPoint);

        Vector3 nextPos = transform.position + moveDirection * navEdgeCheckDistance;
        bool validNavPos = NavMesh.SamplePosition(nextPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas);

        if (!validNavPos || (!agent.pathPending && agent.remainingDistance <= 0.5f))
        {
            patrolTimer += Time.deltaTime;
            agent.isStopped = true;

            if (patrolTimer >= patrolWaitTime)
            {
                PickRandomDirection();
                patrolTimer = 0f;
                agent.isStopped = false;
            }
        }
    }

    private void PickRandomDirection()
    {
        Vector3 bestDirection = -moveDirection;
        float bestDistanceFromEdge = 0f;

        for (int i = 0; i < 20; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0;
            randomDir.Normalize();

            Vector3 testPos = transform.position + randomDir * navEdgeCheckDistance;

            if (NavMesh.SamplePosition(testPos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                if (NavMesh.FindClosestEdge(hit.position, out NavMeshHit edgeHit, NavMesh.AllAreas))
                {
                    if (edgeHit.distance > bestDistanceFromEdge)
                    {
                        bestDistanceFromEdge = edgeHit.distance;
                        bestDirection = (hit.position - transform.position).normalized;
                    }
                }
            }
        }

        moveDirection = bestDirection;
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);
        agent.isStopped = true;

        if (HasLineOfSight() && !soundPlayed)
        {
            PlaySoundEffect();
        }

        if (soundPlayed && !explosionPlayed)
        {
            Playexplosion();
        }

        if (explosionPlayed)
        {
            if (!explosionPrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 origin = transform.position;
        Vector3 direction = (player.position - origin).normalized;
        float distance = Vector3.Distance(origin, player.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
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
            detectionSprite.SetActive(false);
    }

    private void PlaySoundEffect()
    {
        if (beepSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(beepSound);
            soundPlayed = true;
            Invoke(nameof(Playexplosion), beepSound.length);
        }
    }

    private void Playexplosion()
    {
        if (explosionPrefab != null)
        {
            if (meshRenderer != null) meshRenderer.enabled = false;
            if (col != null) col.enabled = false;

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionPlayed = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection * patrolDistance);
    }
}