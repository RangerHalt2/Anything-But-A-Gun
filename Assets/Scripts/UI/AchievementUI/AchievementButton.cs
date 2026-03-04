// Created By: Ryan Lupoli
// Special Button code meant for the Achivement Menu
using UnityEngine;
using UnityEngine.UI;

// Requires the object this script is applied to to be a button
[RequireComponent(typeof(Button))]
public class AchievementButton : MonoBehaviour
{
    [Tooltip("Id for the achievement represented by the button. Must match the ID inside achievements.json exactly. File found in StreamingAssets.")]
    [SerializeField] private string achievementID;
    [Tooltip("Reference to the Icon image for the achievment.")]
    [SerializeField] private Image iconImage;
    [Tooltip("Reference to the AchievementInfoPanel game object")]
    [SerializeField] private AchievementInfoPanel infoPanel;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    void Start()
    {
        RefreshVisual();
    }

    // Called when button is clicked
    void OnClick()
    {
        if(infoPanel != null)
        {
            infoPanel.DisplayAchievement(achievementID);
        }
        else
        {
            Debug.LogWarning("Achievement Button: InfoPanel reference missing.");
        }
    }

    void RefreshVisual()
    {
        Achievement achievement = AchievementManager.Instance.GetAchievement(achievementID);

        if(achievement == null)
        {
            return;
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
}
