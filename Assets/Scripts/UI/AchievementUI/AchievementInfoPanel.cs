// Created By: Ryan Lupoli
// Script designed for the management of the info panel found in the Achievement menu
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInfoPanel : MonoBehaviour
{
    [Header("Text Fields")]
    [Tooltip("Reference to TMPro asset which will display the name of an achivement.")]
    [SerializeField] private TMP_Text nameText;
    [Tooltip("Reference to TMPro asset which will display the tagline of an achivement.")]
    [SerializeField] private TMP_Text flavorText;
    [Tooltip("Reference to TMPro asset which will display the description/unlock condition of an achivement.")]
    [SerializeField] private TMP_Text descriptionText;
    [Tooltip("Reference to TMPro asset which will display the reward an achivement grants.")]
    [SerializeField] private TMP_Text rewardText;
    [Space]
    [SerializeField] private Image iconImage;

    // Default Text displayed by the info panel if the currently selected achivement lacks proper information. Should never be seen in normal gameplay
    [Header("Placeholders")]
    private string namePlaceholder = "Achievement Name Not Found";
    private string flavorPlaceholder = "Achievement Tagline Not Found";
    private string descriptionPlaceholder = "Achievement Desc Not Found";
    private string rewardPlaceholder = "Unlock Reward Not Found";

    [Header("Default Settings")]
    [Tooltip("ID for the achivement which will be displayed by default when the menu is opened")]
    [SerializeField] private string defaultAchivementID;


    void Awake()
    {
        DisplayAchievement(defaultAchivementID);
    }

    // Displays the information for a given achivement to the Info Panel
    public void DisplayAchievement(string id)
    {
        // Look for achivement by ID
        Achievement achievement = AchievementManager.Instance.GetAchievement(id);

        // If no achievment was found with the provided ID, use the placeholders
        if (achievement == null)
        {
            SetPlaceholders();
            return;
        }

        // If achivement.name is null or empty, use placeholder text
        if(string.IsNullOrEmpty(achievement.name))
        {
            nameText.text = namePlaceholder;
        }
        // Otherwise, use the name of the achivement
        else
        {
            nameText.text = achievement.name;
        }


        if (string.IsNullOrEmpty(achievement.flavorText))
        {
            flavorText.text = flavorPlaceholder;
        }
        else
        {
            flavorText.text = achievement.flavorText;
        }

        if (string.IsNullOrEmpty(achievement.description))
        {
            descriptionText.text = descriptionPlaceholder;
        }
        else
        {
            descriptionText.text = achievement.description;
        }

        if (string.IsNullOrEmpty(achievement.reward))
        {
            rewardText.text = rewardPlaceholder;
        }
        else
        {
            rewardText.text = achievement.reward;
        }

        // If achievement is unlocked
        if (achievement.unlocked)
        {
            // Display Icon as normal
            iconImage.color = Color.white;
        }
        else
        {
            // Darken Achievement Icon
            iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }

    void SetPlaceholders()
    {
        nameText.text = namePlaceholder;
        flavorText.text = flavorPlaceholder;
        descriptionText.text = descriptionPlaceholder;
        rewardText.text = rewardPlaceholder;
    }
}
