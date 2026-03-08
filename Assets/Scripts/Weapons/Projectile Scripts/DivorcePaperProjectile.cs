using UnityEngine;

public class DivorcePaperProjectile : MonoBehaviour
{
    public float damage = 20f;
    [SerializeField] private float commonThreshold = 0.5f;
    [SerializeField] private float eliteThreshold = 0.20f;
    [SerializeField] private float bossThreshold = 0.05f;

    public float forwardSpeed = 10f;

    public float largeDrift = 2f;
    public float mediumDrift = 1f;
    public float microDrift = 0.3f;

    public float noiseSpeedLarge = 0.5f;
    public float noiseSpeedMedium = 2f;
    public float noiseSpeedMicro = 8f;
    private float currentDrift;
    private float driftVelocity; // used for smoothing
    private float seedX;
    private float seedY;
    private float seedZ;

    private float currentForwardOffset;
    private float forwardVelocity;

    private float currentVerticalDrift;
    private float verticalVelocity;

    private float timeAlive;


    void Start()
    {
        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
        seedZ = Random.Range(0f, 100f);
    }

    //Entirely dedicated to paper float behavior
    void Update()
    {
        timeAlive += Time.deltaTime;

        float t = Time.time;

        float driftLarge = (Mathf.PerlinNoise(seedX, t * noiseSpeedLarge) - 0.5f) * largeDrift;
        float driftMedium = (Mathf.PerlinNoise(seedY, t * noiseSpeedMedium) - 0.5f) * mediumDrift;
        float driftMicro = (Mathf.PerlinNoise(seedZ, t * noiseSpeedMicro) - 0.5f) * microDrift;

        float targetDrift = driftLarge + driftMedium + driftMicro;

        float forwardNoise = (Mathf.PerlinNoise(seedZ + 100f, t * noiseSpeedMedium) - 0.5f) * 2f;

        float verticalNoise = (Mathf.PerlinNoise(seedX + 200f, t * noiseSpeedMedium) - 0.5f);
        float verticalFalloff = Mathf.Lerp(1f, 0.3f, Mathf.Clamp01(timeAlive * 0.3f));
        float targetVerticalDrift = verticalNoise * mediumDrift * verticalFalloff;

        currentVerticalDrift = Mathf.SmoothDamp(
            currentVerticalDrift,
            targetVerticalDrift,
            ref verticalVelocity,
            0.2f
        );

        float targetForwardOffset = forwardNoise * 2f; // controls how strong backward pull can be

        currentForwardOffset = Mathf.SmoothDamp(
            currentForwardOffset,
            targetForwardOffset,
            ref forwardVelocity,
            0.15f
        );

        // Smooth toward target drift
        currentDrift = Mathf.SmoothDamp(
            currentDrift,
            targetDrift,
            ref driftVelocity,
            0.2f // smoothing time (lower = snappier)
        );

        float modifiedForwardSpeed = forwardSpeed + currentForwardOffset;

        Vector3 forward = transform.forward * modifiedForwardSpeed;
        Vector3 sideDrift = transform.right * currentDrift;
        Vector3 verticalDrift = transform.up * currentVerticalDrift;


        transform.position += (forward + sideDrift + verticalDrift) * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Health enemyHealth = other.gameObject.GetComponentInParent<Health>();
            EnemyClass enemy = other.gameObject.GetComponentInParent<EnemyClass>();
            if(enemyHealth == null)
                Debug.Log("DIVORCE PAPER PROJECTILE - Enemy Health is Null");
            if (enemy == null)
                Debug.Log("DIVORCE PAPER PROJECTILE - Enemy is Null");
            ExecuteEnemy(enemyHealth, enemy);
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }

    private void ExecuteEnemy(Health enemyHealth, EnemyClass enemy)
    {
        float currHealth = enemyHealth.currentHealth;
        float maxHealth = enemyHealth.maxHealth;

        float percentage = currHealth / maxHealth;

        if ((enemy.enemyType == EnemyClass.EnemyType.Common && percentage <= commonThreshold) ||
            (enemy.enemyType == EnemyClass.EnemyType.Elite && percentage <= eliteThreshold) ||
            (enemy.enemyType == EnemyClass.EnemyType.Boss && percentage <= bossThreshold))
        {
            enemyHealth.TakeDamage(enemyHealth.currentHealth, this.transform);
        }
        else
        {
            enemyHealth.TakeDamage(damage, this.gameObject.transform);
        }
            Destroy(this.gameObject);
    }

}
