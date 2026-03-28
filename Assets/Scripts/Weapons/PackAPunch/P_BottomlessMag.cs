using UnityEngine;

public class P_BottomlessMag : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Projectile projCheck = GetComponent<Projectile>();
        if (projCheck != null)
        {
            this.enabled = false;
            return;
        }


        AmmoManager am = GetComponent<AmmoManager>();
        if (am != null)
        {
            am.SetAmmoCapacity(am.maxAmmo);
            am.SetCurrentAmmo(am.reserveAmmo);
            am.SetReserveAmmo(0);
        }
    }
}
