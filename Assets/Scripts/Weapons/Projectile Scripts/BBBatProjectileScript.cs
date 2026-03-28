using Unity.VisualScripting;
using UnityEngine;

public class BBBatProjectileScript : MonoBehaviour
{
    [SerializeField] private GameObject returnalPrefab;
    public GameObject baseWeapon;

    //On Destroy, spawn the returnal 

    void OnDestroy() 
    {
        GameObject returnGameObject = Instantiate(returnalPrefab, transform.position, transform.rotation, null);
        Projectile proj = returnGameObject.GetComponent<Projectile>();
        
        if(GetComponent<P_Explosive>() != null)
        {
            proj.AddComponent<P_Explosive>();
        }

        returnGameObject.GetComponent<BBBatReturnalScript>().baseWeapon = baseWeapon;
    }
}
