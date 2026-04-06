using UnityEngine;

public class MainMenuTemp : MonoBehaviour
{

    private void Start()
    {
        GameObject canvas = GameObject.Find("Master Canvas");
        GameObject player = GameObject.Find("PlayerHolder");

        Destroy(canvas);
        Destroy(player);

    }

}
