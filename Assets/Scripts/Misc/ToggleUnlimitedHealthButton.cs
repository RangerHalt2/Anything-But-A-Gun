using UnityEngine;

public class ToggleUnlimitedHealthButton : MonoBehaviour
{
    private HealthCheats cheats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cheats = GameObject.FindWithTag("Player").GetComponent<HealthCheats>();
    }

    public void OnClick()
    {
        if (cheats != null) 
        {
            cheats.ToggleCheats();
        }
    }
}
