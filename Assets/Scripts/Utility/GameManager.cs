using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Projectile"), LayerMask.NameToLayer("Projectile"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Projectile"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Gun"), LayerMask.NameToLayer("Projectile"), true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
