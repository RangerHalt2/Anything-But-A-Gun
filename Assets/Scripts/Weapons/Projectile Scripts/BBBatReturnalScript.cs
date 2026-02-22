using UnityEngine;

public class BBBatReturnalScript : MonoBehaviour
{
    private Rigidbody rb;
    private Transform player;
    private Projectile proj;
    public GameObject baseWeapon;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").transform;
        proj = GetComponent<Projectile>();
    }

    //Projectile that aims for the player.
    void Update() 
    {
        player = GameObject.FindWithTag("Player").transform;

        rb.linearVelocity = (player.position - transform.position) * proj.speed; //Set it to track the player
    }
    
    void OnTriggerEnter(Collider _other) 
    {
        if (_other.CompareTag("Player")) 
        {
            Destroy(gameObject);
        }
    }

    //When it gets close/hits
    void OnDestroy() 
    {
        baseWeapon.GetComponent<BBBatScript>().launched = false;
        baseWeapon.GetComponent<BBBatScript>().mesh.enabled = true;
    }

}
