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

    void Start()
    {
        // Subscribe to scene loads so we can re-acquire scene objects
        // SceneManager.sceneLoaded += OnSceneLoaded;

        SetupSlider();
    }

    void OnDestroy()
    {
        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find scene objects after every scene load
        volume = null;
        liftGammaGain = null;
        slider = null;
        SetupSlider();
    }
    */

    private void SetupSlider()
    {
        // Find volume if not assigned
        if (volume == null)
            volume = GameObject.Find("Global Volume")?.GetComponent<Volume>();

        if(volume == null)
        {
            Debug.LogWarning("GAMMA SLIDER - No Volume Was Ever Found");
            return;
        }

        // Find slider if not assigned
        if (slider == null)
            slider = GameObject.Find("GSlider")?.GetComponent<Slider>();

        // Get the LiftGammaGain override from the volume profile
        if (volume != null && volume.profile.TryGet(out liftGammaGain))
        {
            Debug.Log("LiftGammaGain found successfully.");
        }
        else
        {
            Debug.LogWarning("Could not find LiftGammaGain on volume profile.");
        }

        // Set up slider
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderChange); // avoid duplicate listeners
            slider.minValue = -1f;
            slider.maxValue = 3f;
            // Load saved gamma, defaulting to 0
            slider.value = PlayerPrefs.GetFloat(PREF_KEY, 0f);
            slider.onValueChanged.AddListener(OnSliderChange);
        }
        else
        {
            Debug.LogWarning("Slider not found.");
        }
    }

    void OnSliderChange(float value)
    {
        ApplyGamma(value);
        PlayerPrefs.SetFloat(PREF_KEY, value);
        PlayerPrefs.Save();
    }

    public void ApplyGamma(float value)
    {
        if (liftGammaGain != null)
            liftGammaGain.gamma.value = new Vector4(1f, 1f, 1f, value);
    }
}