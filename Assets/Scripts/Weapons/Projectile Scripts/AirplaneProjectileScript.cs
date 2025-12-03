using UnityEngine;

public class AirplaneProjectileScript : MonoBehaviour
{

    private float cummulativeDamage;
    [SerializeField] private float baseDamage;
    private WeaponLevel currentWeaponLevel;
    [SerializeField] private float growthRate;
    [SerializeField] private bool dive = false; //So the plane doesn't dive more than once
    [SerializeField] private float diveMultiplier = 2f; //Multiplied by speed to make dive faster.
    private float speed;
    private Rigidbody rb;
    private Transform target;


    [SerializeField] private float sightDistance = 1f; //Distance below that the plane can see an enemy.
    [SerializeField] private float searchRadius = 1f;
    [SerializeField] private LayerMask enemy;

    void Start() 
    {
        speed = GetComponent<Projectile>().speed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dive)
        {
            target = TrackEnemiesBelow();
            if (target != null)
            {
                rb.linearVelocity = Vector3.zero;
                Vector3 direction = target.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
                rb.linearVelocity = transform.forward * speed * diveMultiplier;
                dive = true;
            }
        }
    }

    private Transform TrackEnemiesBelow() //Find if there's an enemy nearby below the airplane
    {
        RaycastHit hit;
        Transform currentTarget = null;
        if (Physics.Raycast(transform.position, -transform.up, out hit, sightDistance))
        { 
            Vector3 hitPoint = hit.point;
            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, searchRadius, enemy);
            
            float shortestDistance = Mathf.Infinity;

            foreach (var hitCollider in hitColliders) 
            {
                if (hitCollider.tag == "Body" || hitCollider.tag == "WeakPoint") 
                {
                    //Find distance between object and transform.position
                    //If the distance is shorter than the current shortest distance, change target.
                    if (Vector3.Distance(hitCollider.transform.position, transform.position) < shortestDistance)
                    { 
                        currentTarget = hitCollider.transform;
                        shortestDistance = Vector3.Distance(hitCollider.transform.position, transform.position);
                    }
                }
            }
        }
        //Return closest enemy transform
        return currentTarget;
    }

    /*void OnDestroy() 
    {
        //Instantiate an explosion
        Debug.Log("BOOM!!");
    }*/

    public void SetWeaponLevelReference(WeaponLevel weaponLevel)
    {
        currentWeaponLevel = weaponLevel;
    }

    public void UpdateLevelDamage()
    {
        cummulativeDamage = baseDamage * Mathf.Pow(growthRate, currentWeaponLevel.Level - 1);
    }
}
