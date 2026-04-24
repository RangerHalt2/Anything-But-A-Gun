using UnityEngine;
using UnityEngine.UI;

public class ToggleUnlimitedHealthButton : MonoBehaviour
{
    private HealthCheats cheats;
    private Toggle self;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheats = GameObject.FindWithTag("Player").GetComponent<HealthCheats>();
        self = GetComponent<Toggle>();
        self.onValueChanged.AddListener(Clicked);
    }

    void Clicked(bool isOn)
    {
        cheats = GameObject.FindWithTag("Player").GetComponent<HealthCheats>();
        if (cheats != null)
        {
            cheats.ToggleCheats();
        }
        else
            Debug.Log("TOGGLE UNLIMITED HEALTH - Health Cheat failed, player cheats were null");
    }
}
