// Created By: Ryan Lupoli
// This is a script meant manage the healthbars used by objects in game
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Tooltip("Slider component from Healthbar Prefab.")]
    [SerializeField] private Slider slider;

    [Tooltip("Gradient of colors which the health bar will use.")]
    [SerializeField] private Gradient gradient;

    [Tooltip("Fill object from Healthbar Prefab.")]
    [SerializeField] private Image fill;

    public void SetMaxHealth(float health)
    {
        // Set Slider to max
        slider.maxValue = health;
        slider.value = health;

        // Update Color
        fill.color = gradient.Evaluate(1f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetHealth(float health)
    {
        // Set the slider's value to the current percentage of health the object has
        slider.value = health;

        // Update Color
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
