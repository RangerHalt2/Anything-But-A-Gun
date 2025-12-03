using UnityEngine;

public class ToggleUnlimitedAmmoButton : MonoBehaviour
{
    private WeaponHandler cheats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheats = GameObject.FindWithTag("Player").GetComponent<WeaponHandler>();
    }

    public void OnClick()
    {
        if (cheats != null) 
        {
            cheats.ToggleCheats();
        }
    }
}
