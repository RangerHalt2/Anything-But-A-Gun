// Created By: Ryan Lupoli
// Special Button code meant for the Achivement Menu
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    void OnEnable()
    {
        RefreshVisual();
    }

    // Called when button is clicked
    void OnClick()
    {
        // Check if the player is holding ctrl or shift when pressing an achievement button
        bool ctrl = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
        bool shift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;

        // Debug: Toggle Achievment
        // If the Achievement Manager is set to allow for toggling achivement states, and the player presses a button while holding ctrl and shift, toggle the achievement
        if(AchievementManager.Instance.canToggle() && ctrl && shift)
        {
            AchievementManager.Instance.ToggleAchievement(achievementID);
            RefreshVisual();
        }


        if (infoPanel != null)
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
