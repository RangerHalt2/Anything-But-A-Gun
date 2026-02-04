using UnityEngine;
using UnityEngine.ProBuilder;

public class P_Explosive : MonoBehaviour
{

    private bool isProjectile = true;
    private Projectile projectile;

    void Start()
    {
        WeaponClass wc = gameObject.GetComponent<WeaponClass>();
        if (wc != null)
        {
            //wc.isProjectile = isProjectile;
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
            projectile = gameObject.GetComponent<Projectile>();
            projectile.SetIsAoE(true);
        }
    }

}
