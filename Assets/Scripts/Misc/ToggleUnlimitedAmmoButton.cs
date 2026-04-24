using UnityEngine;
using UnityEngine.UI;

public class ToggleUnlimitedAmmoButton : MonoBehaviour
{
    private WeaponHandler cheats;
    private Toggle self;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheats = GameObject.FindWithTag("Player").GetComponent<WeaponHandler>();
        self = GetComponent<Toggle>();
        self.onValueChanged.AddListener(Clicked);
    }

    void Clicked(bool isOn)
    {
        if (cheats != null) 
        {
            cheats.ToggleCheats();
        }
    }
}
