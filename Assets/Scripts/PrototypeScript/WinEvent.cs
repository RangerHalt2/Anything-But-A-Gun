using UnityEngine;

public class WinEvent : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject inGameUI;

    public bool hasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            hasWon = true;
            winScreen.SetActive(true);
            inGameUI.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

}
