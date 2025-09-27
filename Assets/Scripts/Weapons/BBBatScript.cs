using UnityEngine;
using System.Collections;

public class BBBatScript : MonoBehaviour, IWeapon
{
    public int health = 10; //Health of the melee weapon

    public bool striking = false;

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
            //_other.gameObject.GetComponent<EnemyController>().LoseLife(); Make enemy/damageable lose life
            Debug.Log("Oh hey!, an enemy!");
            health--;
        }
    }

    public void Shoot() //Required method for interface IWeapon
    {
        if (!striking)
        {
            StartCoroutine(CLASH());
            //Animate Attack
        }
    }

    IEnumerator CLASH()
    {
        striking = true;
        yield return new WaitForSeconds(2);
        striking = false;
    }

    public void Reload()
    {
    }
}
