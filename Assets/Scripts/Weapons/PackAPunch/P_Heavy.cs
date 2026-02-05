using UnityEngine;
using UnityEngine.ProBuilder;

public class P_Heavy : MonoBehaviour
{

    private Projectile projectile;

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
                Debug.LogError("");
            }
        }
    }

    

}
