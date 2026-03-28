using UnityEngine;

public class SnapshotBehaviorScript : MonoBehaviour
{
    public GameObject copyOf; //Whatever this snapshot is in reference to.

    [SerializeField] private Health theirHealth; //Health of enemy
    [SerializeField] private float intermediaryHealth; //Compares each of the other two healths to keep snapshot's health equal to the enemy.
    [SerializeField] private Health myHealth; //Health of snapshot

    [SerializeField] private BoxCollider myHead;
    [SerializeField] private BoxCollider myBody;

    [SerializeField] private BoxCollider theirHead;
    [SerializeField] private BoxCollider theirBody;

    [SerializeField] private bool myHealthUnchanged;
    [SerializeField] private bool theirHealthUnchanged;

    public float snapshotTimer = 20f; //The amount of time that the snapshot sticks around.

    [SerializeField] private bool timesUp = false;

    void OnEnable()
    {
        if (copyOf != null)
        {
            //Regulate the colliders to match the copy as well.
            myHead = gameObject.transform.Find("Head").GetComponent<BoxCollider>();
            myBody = gameObject.transform.Find("Body").GetComponent<BoxCollider>();

            theirHead = copyOf.transform.Find("Head").GetComponent<BoxCollider>();
            theirBody = copyOf.transform.Find("Body").GetComponent<BoxCollider>();

            myHead.center = theirHead.center;
            myHead.size = theirHead.size;

            myBody.center = new Vector3(theirBody.center.x, theirBody.center.y, theirBody.center.z);
            myBody.size = new Vector3(theirBody.size.x, theirBody.size.y, theirBody.size.z);

            theirHealth = copyOf.GetComponent<Health>();
            myHealth = GetComponent<Health>();

            myHealth.maxHealth = theirHealth.maxHealth;
            myHealth.currentHealth = theirHealth.currentHealth;
            myHealth.teamID = theirHealth.teamID;

            //Havent regulated collider size yet, that's why they're floating. Fix later.

            intermediaryHealth = myHealth.currentHealth;

            //Make this object a virtual copy of the original
            gameObject.GetComponent<SpriteRenderer>().sprite = copyOf.GetComponent<SpriteRenderer>().sprite;
            gameObject.transform.localScale = copyOf.transform.localScale;

        }
    }

    // Update is called once per frame
    void Update()
    {

        if (copyOf != null)
        {
            snapshotTimer -= 1 * Time.deltaTime;
            if (snapshotTimer < 0)
            {
                timesUp = true;
                Destroy(gameObject);
            }

            if (intermediaryHealth == myHealth.currentHealth) { myHealthUnchanged = true; }
            else { myHealthUnchanged = false; }
            if (theirHealth.currentHealth == intermediaryHealth) { theirHealthUnchanged = true; }
            else { theirHealthUnchanged = false; }

            if (myHealthUnchanged && theirHealthUnchanged)
            {
                //Nothing. If they're all equal nothing should be happening
            }
            else
            {
                if (theirHealthUnchanged && (intermediaryHealth > myHealth.currentHealth)) //If my health decreased
                {
                    theirHealth.TakeDamage(intermediaryHealth - myHealth.currentHealth, this.transform); //Decrease their health
                    intermediaryHealth = myHealth.currentHealth;
                }
                else if (myHealthUnchanged && (intermediaryHealth > theirHealth.currentHealth)) //If their health decreased
                {
                    myHealth.TakeDamage(intermediaryHealth - theirHealth.currentHealth, this.transform); //Decrease my health
                    intermediaryHealth = theirHealth.currentHealth;
                }
                else if (theirHealthUnchanged && intermediaryHealth < myHealth.currentHealth) //If my health increased
                {
                    theirHealth.ReceiveHealing(myHealth.currentHealth - intermediaryHealth); //Increase their health
                    intermediaryHealth = myHealth.currentHealth;
                }
                else if (myHealthUnchanged && intermediaryHealth < theirHealth.currentHealth) //If their health decreased
                {
                    myHealth.ReceiveHealing(theirHealth.currentHealth - intermediaryHealth); //Increase my health
                    intermediaryHealth = theirHealth.currentHealth;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (!timesUp) 
        {
            if (theirHealth != null) 
            {
                theirHealth.Die();
            }
        }
    }
}
