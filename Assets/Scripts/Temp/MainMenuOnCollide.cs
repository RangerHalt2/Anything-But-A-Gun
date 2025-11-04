using UnityEngine;

public class MainMenuOnCollide : MonoBehaviour
{
    private SceneController controller;

    private void Start()
    {
        controller = GameObject.FindAnyObjectByType<SceneController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            controller.GoToScene("Title Screen");
        }
    }
}
