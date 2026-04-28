using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] private float delay = 3f;

    private void Start()
    {
        if(delay < 0f) delay = 2f;
        Invoke(nameof(DelayedStart), delay);
    }


    private void DelayedStart()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
