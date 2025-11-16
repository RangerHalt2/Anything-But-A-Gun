using UnityEngine;

public class PossessedEffect : MonoBehaviour
{
    private EnemyAI enemy;
    private CentralEnemyManager enemyManager;

    void Start() 
    {
        enemyManager = CentralEnemyManager.Instance;
        enemy = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        enemy.whatIsPlayer = LayerMask.GetMask("Enemy");
        enemy.player = enemyManager.FindClosestEnemy(enemy.gameObject.transform.position).transform;
    }
}
