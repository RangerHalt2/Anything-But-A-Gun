using UnityEngine;

public class P_FamilyPhoto : MonoBehaviour
{
    private float lifeStealPercentage = 0.15f;
    


    public void SendHealing(float damageAmount)
    {
        PlayerController pc = GameObject.FindAnyObjectByType<PlayerController>();
        if (pc != null)
        {
            Health health = pc.GetComponent<Health>();
            if (health != null)
            {
                float healValue = damageAmount * lifeStealPercentage;
                health.ReceiveHealing(healValue);
            }
        }
    }
}
