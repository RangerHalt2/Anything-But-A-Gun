using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    private TextMeshProUGUI textmeshpro;
    private float pollingTime = 1.5f;
    private float time;
    private int frameCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
         textmeshpro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            textmeshpro.text = frameRate.ToString() + " FPS";
            time -= pollingTime;
            frameCount = 0;
        }

    }


}
