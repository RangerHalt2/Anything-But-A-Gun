using UnityEngine;

public class unlockmouse : MonoBehaviour
{
    public GameObject GameOverScreen;

    void Update()
    {
        if (GameOverScreen.activeInHierarchy)
        {
           
            Cursor.lockState = CursorLockMode.None;
           
            Cursor.visible = true;
        }
        else
        {
           
            Cursor.lockState = CursorLockMode.Locked;
        
            Cursor.visible = false;
        }
    }
}