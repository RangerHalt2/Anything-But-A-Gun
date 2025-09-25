using UnityEngine;

public class BBBatScript : MonoBehaviour , IWeapon
{
    public int health = 10; //Health of the melee weapon

    private bool striking = false;

    void Update()
    {
        if (health <= 0)
        {
            //Change visual to Broken BBBat
        }
    }

    void OnTriggerEnter(Collider _other) //Detect if an enemy is near
    {
        if (_other.tag == "Enemy" && striking) // If it's an enemy, the bat has health, and the player presses shoot, attack the enemy
        {
            //_other.gameObject.GetComponent<EnemyController>().LoseLife(); Make him lose life
            Debug.Log("Oh hey!, an enemy!");
            health--;

            striking = false;
        }
        else 
        {
            striking = false;
        }
    }

    public void Shoot() //Required method for interface IWeapon
    {
        //Animate Attack
        striking = true;
        //Deal damage to enemy (Don't have yet)
    }
}
