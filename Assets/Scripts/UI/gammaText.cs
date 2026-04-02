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
        valueText.text = value.ToString("F2");
    }
}