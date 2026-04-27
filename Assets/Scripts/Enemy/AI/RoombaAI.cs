using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class RoombaAI : EnemyClass
{
    //[Header("Target Layers and Player")]
    //[SerializeField] private NavMeshAgent agent;
    //[SerializeField] private Transform player;
    //[SerializeField] private LayerMask whatIsPlayer;

    [Header("NavMesh Patrol Settings")]
    [SerializeField] private float patrolDistance = 10f;
    [SerializeField] private float patrolSpeed = 3f;
    //[SerializeField] private float patrolWaitTime = 1.5f;
    [SerializeField] private float navEdgeCheckDistance = 2f;

    private Vector3 moveDirection;
    //private float patrolTimer;

    //[Header("Sight and Attack Range")]
    //[SerializeField] private float sightRange = 10f;
    //[SerializeField] public float attackRange = 2f;
    //[SerializeField] private bool playerInSightRange;
    //[SerializeField] public bool playerInAttackRange;

    [Header("Investigation Settings")]
    private Vector3 lastKnownPlayerPosition;
    private bool isInvestigating;
    private float investigationWaitTime = 3f;
    private float investigationTimer;

    //[Header("Detection Indicator")]
    //[SerializeField] private GameObject detectionSprite;
    //[SerializeField] private float detectionDisplayTime = 1f;
    //private bool hasShownDetectionSprite = false;

    [Header("Sound and Visual Effects")]
    [SerializeField] private GameObject explosionsfx;
    [SerializeField] private GameObject explosionPrefab;
    private bool soundPlayed = false;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float explosionDelay = 2f;

    [Header("Team Settings")]
    [SerializeField] private int teamID = 0;

    private bool isExploding = false;
    private bool hasExploded = false;

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

        /*if (audioSource == null)
            audioSource = GetComponent<AudioSource>();*/
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
    
    override
    public void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    override
    public void Attacking()
    {
        agent.isStopped = true;
        agent.SetDestination(transform.position);

        if (!isExploding)
        {
            isExploding = true;
            StartCoroutine(ExplodeAfterDelay());
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);

        /*if (!soundPlayed)
            PlaySoundEffect();*/

        //float delay = (audioSource != null && audioSource.clip != null) ? audioSource.clip.length : 0f;
        //yield return new WaitForSeconds(delay);

        PlayExplosion();
    }

    /*private void PlaySoundEffect()
    {
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();

        soundPlayed = true;
    }*/

    private void PlayExplosion()
    {
        if (explosionPrefab != null && !hasExploded)
        {
            hasExploded = true;

            if(explosionsfx != null)
                Instantiate(explosionsfx, transform.position, transform.rotation, null);

            if (meshRenderer != null) meshRenderer.enabled = false;
            if (col != null) col.enabled = false;

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider nearbyObject in colliders)
            {
                Health health = nearbyObject.GetComponent<Health>();
                if (health != null)
                    DoDamage(health);
            }

            Die();

            Destroy(gameObject);
        }
    }

    private void DoDamage(Health health)
    {
        if (health.teamID != teamID)
            health.TakeDamage(explosionDamage, this.gameObject.transform);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection * patrolDistance);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}