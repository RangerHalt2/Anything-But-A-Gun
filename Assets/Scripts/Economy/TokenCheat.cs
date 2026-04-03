using UnityEngine;

public class TokenCheat : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public EconomyManager cheat; // Reference to the EconomyManager script
    public GameObject MButton; // Reference to the button that will trigger the cheat
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnButtonPress()
    {
        cheat.PTOAmount += 1000;
    }

}
