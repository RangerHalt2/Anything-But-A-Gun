using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private Image splashScreen;
    private float timer = 0f;

    private void Start()
    {
        if (delay < 0f) delay = 2f;
        Invoke(nameof(DelayedStart), delay);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (splashScreen != null)
        {
            Color color = splashScreen.color;
            if (timer <= (delay / 2))
                color.a = Mathf.Lerp(0f, 1f, timer / (delay / 2));
            if (timer > (delay / 2))
                color.a = Mathf.Lerp(1f, 0f, timer / (delay / 2));
            splashScreen.color = color;
        }
    }


    private void DelayedStart()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
