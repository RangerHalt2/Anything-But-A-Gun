using System.Collections;
using UnityEngine;

public class JoustingColliderScript : MonoBehaviour
{
    private JoustingHorseScript jHS;
    public float damage;
    public int myTeamID = 0;
    private PlayerController pc;

    void Start() 
    {
        jHS = GetComponentInParent<JoustingHorseScript>();
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (jHS != null && !pc.canDash) 
        {
            if (collision.collider.gameObject.tag == "Body")
            {
                Debug.Log("Collided with: " + collision.collider.gameObject);
                Health potentialHit = collision.collider.gameObject.GetComponentInParent<Health>();
                if (potentialHit != null && potentialHit.teamID != myTeamID)
                {
                    potentialHit.TakeDamage(damage);
                }
            }
        }
    }
}
