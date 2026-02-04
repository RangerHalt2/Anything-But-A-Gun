using UnityEngine;

public class P_Growing : MonoBehaviour
{

    private bool isProjectile = true;
    private float maxSize = 4f; //As a percentage, 1.5f allows for 50% bigger max
    private float maxDamageIncrease = 2f; //The max damage , the base obviously being 1x. This is also in percentage.
    private float timeToMax = 1.5f; //Time in seconds to hitting max damage.
    private float elapsedTime = 0;

    private Projectile projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WeaponClass wc = gameObject.GetComponent<WeaponClass>();
        if(wc != null)
        {
            //wc.isProjectile = isProjectile;
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
            projectile = gameObject.GetComponent<Projectile>();
        }
    }


    private void Update()
    {
        if (elapsedTime < timeToMax)
        {
            float currProg = elapsedTime / timeToMax;
            float multi = Mathf.Lerp(1f, maxDamageIncrease, currProg);
            float size = Mathf.Lerp(1f, maxSize, currProg); //The assumption is the base scale is 1, 1, 1.
            gameObject.transform.localScale = new Vector3(size, size, size); // Increase the scale up to max.
            if(projectile != null)
                projectile.externalDmgMod = multi;
            elapsedTime += Time.deltaTime;
        }
    }

}
