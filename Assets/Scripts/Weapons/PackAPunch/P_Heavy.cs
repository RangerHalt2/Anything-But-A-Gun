using UnityEngine;
using UnityEngine.ProBuilder;

public class P_Heavy : MonoBehaviour
{

    private Projectile projectile;
    private float gravity = 2.5f;
    private float extraDamage = 1.5f; //50% more damage;

    private void Start()
    {
        WeaponClass wc = gameObject.GetComponent<WeaponClass>();
        if(wc != null)
        {
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
            projectile = gameObject.GetComponent<Projectile>();
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("P_HEAVY - Rigidbody is null");
                return;
            }
            rb.useGravity = true;
            projectile.SetGravityScale(gravity);
            projectile.externalDmgMod = extraDamage;
        }
    }

}
