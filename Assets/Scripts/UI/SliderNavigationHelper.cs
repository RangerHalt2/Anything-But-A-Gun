using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderNavigationHelper : MonoBehaviour
{
    private Slider slider;
    private bool isAdjusting = false;

    private Navigation originalNavigation;

    void Start()
    {
        slider = GetComponent<Slider>();
        originalNavigation = slider.navigation; // cache the inspector values
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject) return;

        if (Input.GetButtonDown("Submit"))
        {
            isAdjusting = !isAdjusting; // toggle in/out of adjust mode
            Navigation nav = slider.navigation;
            nav.mode = isAdjusting ? Navigation.Mode.None : originalNavigation.mode;

            if (!isAdjusting)
                slider.navigation = originalNavigation; // restore the full cached version
            else
            {
                nav.mode = Navigation.Mode.None;
                slider.navigation = nav;
            }
        }

        if (isAdjusting)
        {
            float input = Input.GetAxis("Horizontal");
            slider.value += input * Time.deltaTime; // tune the multiplier to taste
        }
    }
}
