using UnityEngine;

public class PackAPunchMachine : MonoBehaviour, IInteractable
{
    private WeaponHandler wh;
    private WeaponClass wc;

    public void Interact()
    {
        Debug.Log("Pack-A-Punch Machine Begin");
        wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        wc = wh.currentWeapon.GetComponent<WeaponClass>();

        if(wc != null && wc.GetPackAPunchIndex() == -1)
        {
            Debug.Log("Pack-A-Punch Machine determined it can be added");
            int selectedPunch = Random.Range(0, wc.GetPackAPunchLength());
            wc.SetPackAPunchIndex(selectedPunch);
            wc.AddPackAPunch();
        }
    }
}
