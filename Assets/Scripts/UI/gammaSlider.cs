using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gammaSlider : MonoBehaviour
{

    public Volume volume;

    private LiftGammaGain liftGammaGain;


    public Slider slider;



    private const string PREF_KEY = "gamma";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (volume.profile.TryGet(out liftGammaGain))



            slider.minValue = -1f;
        slider.maxValue = 3f;
        slider.value = 0f;

        slider.onValueChanged.AddListener(OnSliderChange);

        // setting slider value for starting up
    }


    private void Update()
    {
        if (volume = null) ;
        {
            volume = GameObject.Find("Global Volume")?.GetComponent<UnityEngine.Rendering.Volume>();
        }
    }


    void OnSliderChange(float value)
    {
        ApplyGamma(value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
        PlayerPrefs.Save();
    }
    // keeps the gamma value updated and saves it to player prefs so it can be loaded in the future
    public void ApplyGamma(float value)
    {
        if (liftGammaGain != null)


            liftGammaGain.gamma.value = new Vector4(1f, 1f, 1f, value);

    }
}
  