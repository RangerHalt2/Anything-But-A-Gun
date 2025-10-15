using UnityEngine;

public class EnemyManagerCooperation : MonoBehaviour
{
    [SerializeField] private CentralEnemyManager enemyManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //NOTE: THE REASON THIS WORKS IS BECAUSE THE ENEMIES START IN THE SCENE. IF THEY END UP BEING CREATED INSTEAD OF STARTING, CHANGE THIS IMMEDIATELY.
    void Start()
    {
        enemyManager = CentralEnemyManager.Instance;
        enemyManager.RegisterEnemy(this.gameObject);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        enemyManager.UnregisterEnemy(this.gameObject);
    }
}
