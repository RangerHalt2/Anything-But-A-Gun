using UnityEngine;

public class PossessedEffect : MonoBehaviour
{
    public float timer = 10f;
    private EnemyAI enemy;
    private Health health;
    private CentralEnemyManager enemyManager;

    void Start() 
    {
        enemyManager = CentralEnemyManager.Instance;
        enemy = GetComponent<EnemyAI>();
        enemy.whatIsPlayer = LayerMask.GetMask("Enemy");
        health.teamID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1 * Time.deltaTime;
        if (timer < 0) 
        {
            Destroy(gameObject);
        }
        enemy.player = enemyManager.FindClosestEnemy(enemy.gameObject.transform.position).transform;
    }
}
