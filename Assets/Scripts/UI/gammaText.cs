using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class gammaText : MonoBehaviour
{
    [SerializeField] private Slider mainSlider;
    [SerializeField] private TextMeshProUGUI valueText;

    private void Start()
    {

        mainSlider.onValueChanged.AddListener(UpdateSliderText);
    }

    public void UpdateSliderText(float value)
    {
        float modifedValue = value + 1f;
        valueText.text = modifedValue.ToString("F2");
    }

}