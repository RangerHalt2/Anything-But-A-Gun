using System.Collections;
using UnityEngine;

public class P_ShiningArmor : MonoBehaviour
{
    private float lifeTime = 3f;
    private bool isProjectile = false;

    private void Start()
    {
        if (!isProjectile)
        {
            WeaponClass wc = gameObject.GetComponent<WeaponClass>();
            if (wc != null)
            {
                this.enabled = true;
            }
            else
            {
                this.enabled = false;
            }
        }
    }

    public void ApplyArmor()
    {
        PlayerController playerController = GameObject.FindAnyObjectByType<PlayerController>();
        Health health = playerController.gameObject.GetComponent<Health>();

        health.ShiningArmor = true;

        StartCoroutine(RemoveArmor());

        return;
    }

    private IEnumerator RemoveArmor()
    {
        yield return new WaitForSeconds(lifeTime);
        PlayerController playerController = GameObject.FindAnyObjectByType<PlayerController>();
        Health health = playerController.gameObject.GetComponent<Health>();

        health.ShiningArmor = false;
    }
}
