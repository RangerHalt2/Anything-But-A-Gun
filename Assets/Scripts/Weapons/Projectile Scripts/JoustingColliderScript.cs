using System.Collections;
using UnityEngine;

public class JoustingColliderScript : MonoBehaviour
{
    private JoustingHorseScript jHS;
    public float damage;
    public int myTeamID;
    public float currentSpeed;
    private Vector3 lastPosition;

    void Start() 
    {
        jHS = GetComponentInParent<JoustingHorseScript>();
    }

    void Update() 
    {
        if (jHS != null && jHS.JOUSTING)
        {
            float distance = Vector3.Distance(transform.position, lastPosition);
            currentSpeed = distance / Time.deltaTime;
            lastPosition = transform.position;
        }
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (jHS != null && jHS.JOUSTING) 
        {
            if (collision.collider.gameObject.tag == "Body")
            {
                Debug.Log("Collided with: " + collision.collider.gameObject);
                Health potentialHit = collision.collider.gameObject.GetComponentInParent<Health>();
                if (potentialHit != null && potentialHit.teamID != myTeamID)
                {
                    potentialHit.TakeDamage(damage * currentSpeed);
                }
            }
        }
    }
}
