using UnityEngine;
using UnityEngine.SceneManagement;

public class WinEvent : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject inGameUI;

    public bool hasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("second floor");
    }

}
