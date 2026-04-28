using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenHandler : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    private float halfDelay;
    [SerializeField] private Image splashScreen;
    private float timer = 0f;

    private void Start()
    {
        if (delay < 0f) delay = 2f;
        halfDelay = delay / 2f;
        Invoke(nameof(DelayedStart), delay);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (splashScreen != null)
        {
            Color color = splashScreen.color;
            if (timer <= halfDelay)
                color.a = Mathf.Lerp(0f, 1f, timer / halfDelay);
            else if (timer <= halfDelay * 1.1f)
                color.a = 1f;
            else
            {
                float t = (timer - halfDelay) / halfDelay;
                color.a = Mathf.Lerp(1f, 0f, t);
            }
            splashScreen.color = color;
        }
    }


    private void DelayedStart()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
