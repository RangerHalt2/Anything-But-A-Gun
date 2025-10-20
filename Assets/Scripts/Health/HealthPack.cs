using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private float healAmmount = 0f;

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();

        Heal(health);
    }

    //Whatever trigger we decide can simply call this function and it will heal the player via the health pack amount
    //Similarly, this code can be used for healing orbs dropped on the ground
    private void Heal(Health health)
    {
        if (health != null && health.teamID == 0)
        {
            health.ReceiveHealing(healAmmount);
            Destroy(gameObject);
        }
    }

}
