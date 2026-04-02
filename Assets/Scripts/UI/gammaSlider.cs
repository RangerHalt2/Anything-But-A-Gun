using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class gammaSlider : MonoBehaviour
{

    public PostProcessVolume volume;

    public Slider slider;

    private ColorGrading colorGrading;

   
    private const string PREF_KEY = "gamma";

    public TextMeshProUGUI gammaValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (slider == null) return;
        if (volume == null) volume = FindAnyObjectByType<PostProcessVolume>();
        if (volume == null || volume.profile == null) return;

        volume.profile.TryGetSettings(out colorGrading);
        if(colorGrading == null) return;

        // because i hate to break the game but this way it can try to find missing components shoutout unity help forums

        slider.minValue = 0.5f;
        slider.maxValue = 2f;
        slider.value = 1f;
        gammaValueText.text = slider.value.ToString();

        slider.onValueChanged.AddListener(OnSliderChange);

        // setting slider value for starting up
    }

    // Update is called once per frame
    void OnSliderChange(float value)
    {
       ApplyGamma(value);
       PlayerPrefs.SetFloat(PREF_KEY, value);
       PlayerPrefs.Save();
    }
    // keeps the gamma value updated and saves it to player prefs so it can be loaded in the future
    public void ApplyGamma(float value)
    {
        if (colorGrading == null) return;
        colorGrading.postExposure.value = value - 1;
        gammaValueText.text = value.ToString();

    }
}
