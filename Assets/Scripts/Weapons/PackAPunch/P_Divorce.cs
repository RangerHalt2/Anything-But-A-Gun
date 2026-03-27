using UnityEngine;

public class P_Divorce : MonoBehaviour
{
    [SerializeField] private float commonThreshold = 0.3f;
    [SerializeField] private float eliteThreshold = 0.15f;
    [SerializeField] private float bossThreshold = 0.05f;

    private Projectile projectile;

    // Disable if it's not a projectile
    void Start()
    {
        WeaponClass wc = gameObject.GetComponent<WeaponClass>();
        if (wc != null)
        {
            //wc.isProjectile = isProjectile;
            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Health enemyHealth = other.gameObject.GetComponentInParent<Health>();
            EnemyClass enemy = other.gameObject.GetComponentInParent<EnemyClass>();
            if (enemyHealth == null)
                Debug.Log("DIVORCE PAPER Promotion - Enemy Health is Null");
            if (enemy == null)
                Debug.Log("DIVORCE PAPER Promotion - Enemy is Null");
            ExecuteEnemy(enemyHealth, enemy);
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
    }

}
