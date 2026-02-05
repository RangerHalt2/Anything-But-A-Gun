//LB: Note, the parentWeapon being a WeaponClass appears to be redundant, referencing the root gameObject would probably cut the middle man. Low Priority Change, return later.

using UnityEngine;

public class P_Climactic : MonoBehaviour
{

    public WeaponClass parentWeapon;

    private float baseDamageMulti = 0.7f; //30% dmg reduction
    private float middleDamageMulti = 1.5f; //50% dmg increase
    private float finalShotDamageMulti = 4f; //200% dmg increase

    private float middlePercentage = 0.40f; //40% of the shots?
    private float lastPercentage = 0.10f; //10% of the shots?

    private void Start()
    {
        WeaponClass wc = gameObject.GetComponent<WeaponClass>();
        if (wc != null)
        {
            this.enabled = false;
        }
        else
        {
            if (parentWeapon != null)
            {
                AmmoManager am = parentWeapon.gameObject.GetComponent<AmmoManager>();
                Projectile projectile = gameObject.GetComponent<Projectile>();
                if (am != null && projectile != null)
                {
                    int currAmmo = am.GetCurrentAmmo();
                    int capAmmo = am.GetAmmoCapacity();
                    float currPercentage = (float) currAmmo / capAmmo;

                    float retMulti = 1.0f;

                    if(currPercentage > middlePercentage )
                    {
                        retMulti = baseDamageMulti;
                    }
                    if (currPercentage <= middlePercentage && currPercentage > lastPercentage)
                    {
                        retMulti = middleDamageMulti;
                    }
                    if(lastPercentage >= currPercentage)
                    {
                        retMulti = finalShotDamageMulti;
                    }

                    projectile.externalDmgMod = retMulti;

                }
            }
        }//end of else
    }//end of Start

}
