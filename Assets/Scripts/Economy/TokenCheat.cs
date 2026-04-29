using UnityEngine;
using UnityEngine.UI;

public class TokenCheat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EconomyManager cheat; // Reference to the EconomyManager script
    public GameObject MButton; // Reference to the button that will trigger the cheat
    private Toggle self;

    void Start()
    {
        self = GetComponent<Toggle>();
        self.onValueChanged.AddListener(Clicked);
    }

    // Update is called once per frame
    void Update()
    {

    }


    void Clicked(bool isOn)
    {
        EconomyManager eco = GameObject.FindAnyObjectByType<EconomyManager>();
        eco.ToggleInfinitePTO();
    }

}
