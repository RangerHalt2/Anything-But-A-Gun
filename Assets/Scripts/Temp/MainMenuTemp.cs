using UnityEngine;

public class MainMenuTemp : MonoBehaviour
{

    private void Start()
    {
        GameObject canvas = GameObject.Find("Master Canvas");
        GameObject player = GameObject.Find("PlayerHolder");

        Destroy(canvas);
        Destroy(player);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CleanupDontDestroyOnLoad.DestroyAllDontDestroyOnLoad();

        Invoke(nameof(FlickerCanvas), 0.1f);

    }

    private void FlickerCanvas()
    {
        UIManager mainCanvas = GameObject.FindAnyObjectByType<UIManager>();
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(false);
            mainCanvas.gameObject.SetActive(true);
        }
    }

}
