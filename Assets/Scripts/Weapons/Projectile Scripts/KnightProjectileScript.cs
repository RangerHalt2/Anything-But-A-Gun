using UnityEngine;

public class KnightProjectileScript : MonoBehaviour
{

    private float cummulativeDamage;
    [SerializeField] private float baseDamage;
    private WeaponLevel currentWeaponLevel;
    [SerializeField] private float growthRate;
    [SerializeField] private float sightDistance = 100f; //Distance on either side that the knight can turn.
    [SerializeField] private bool turned = false; //So the knight doesn't turn more than once
    private float speed;
    private Rigidbody rb;

    void Start() 
    {
        speed = GetComponent<Projectile>().speed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!turned)
        {
            RaycastHit hitLeft;
            Physics.Raycast(transform.position, -transform.right, out hitLeft, sightDistance);
                if (hitLeft.collider != null && (hitLeft.collider.gameObject.tag == "Body" || hitLeft.collider.gameObject.tag == "WeakPoint"))
                {
                    rb.linearVelocity = Vector3.zero;
                    Vector3 direction = hitLeft.collider.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(direction);
                    rb.linearVelocity = transform.forward * speed;
                    turned = true;
                }
            
        }
        if (!turned) 
        { 
            RaycastHit hitRight;
            Physics.Raycast(transform.position, transform.right, out hitRight, sightDistance);
            if (hitRight.collider != null && (hitRight.collider.gameObject.tag == "Body" || hitRight.collider.gameObject.tag == "WeakPoint"))
            {
                    rb.linearVelocity = Vector3.zero;
                    Vector3 direction = hitRight.collider.transform.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(direction);
                    rb.linearVelocity = transform.forward * speed;
                    turned = true;
                
            }
        }
    }

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
    }

    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level - 1);
    }
}
