// Created By: Ryan Lupoli
// Manages the Pop-up which appear after an achievement is earned
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementPopUp : MonoBehaviour
{
    [Tooltip("The game object for the Achievment Pop-Up")]
    [SerializeField] private GameObject achievementPopUp;

    [Header("Position Settings")]
    [Tooltip("The position of the Acvievment Pop-Up when it is off screen. This should be outside of the area on the canvas visible by the camera.")]
    [SerializeField] Vector2 offScreenPos;
    [Tooltip("The position of the Acvievment Pop-Up when it is on screen. This should be clearly visible by the camera.")]
    [SerializeField] Vector2 onScreenPos;

    [Header("Time Settings")]
    [Tooltip("The amount of time the Achievement Pop-up will stay on screen. This ignores the time it takes for the Pop-up to Lerp on/off screen.")]
    [SerializeField] private float stayTime;
    [Tooltip("The amount of time it takes for the Pop-up to move on/off of the screen. Used by the LERP function/")]
    [SerializeField] private float transitionTime;

    private RectTransform rect;

    [Header("Audio Settings")]
    [Tooltip("Audio Effect to play whenever an achievment is earned.")]
    [SerializeField] private GameObject achievementSFX;

    [Header("Info Fields")]
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

    [Header("Placeholders")]
    private string namePlaceholder = "Achievement Name Not Found";
    private string flavorPlaceholder = "Achievement Tagline Not Found";
    private string descriptionPlaceholder = "Achievement Desc Not Found";
    private string rewardPlaceholder = "Unlock Reward Not Found";

    private Queue<string> achievementQueue = new Queue<string>();
    private bool isDisplaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = achievementPopUp.GetComponent<RectTransform>();
        rect.anchoredPosition = offScreenPos;
    }


    // Moves the achievement Pop-Up on screen and populates its data with the most recently earned achievement
    private void DisplayPopUp()
    {
        string id = AchievementManager.Instance.GetLastAchievement();

        if (!string.IsNullOrEmpty(id))
        {
            achievementQueue.Enqueue(id);
        }

        if (!isDisplaying)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (achievementQueue.Count > 0)
        {
            string achievementID = achievementQueue.Dequeue();

            PopulateData(achievementID);
            Instantiate(achievementSFX);

            yield return StartCoroutine(PopUpRoutine());
        }

        isDisplaying = false;
    }

    #region PopUp Location Management
    // Basic Coroutine which allows for the popup to wait for the defined stay time
    private IEnumerator PopUpRoutine()
    {
        Debug.Log("AchievmenetPopUp: Started Pop-Up Coroutine");
        //Slide in
        yield return StartCoroutine(MovePopup(offScreenPos, onScreenPos));

        // Stay on Screen
        yield return new WaitForSeconds(stayTime);

        // Slide out
        yield return StartCoroutine(MovePopup(onScreenPos, offScreenPos));
    }

    private IEnumerator MovePopup(Vector2 start, Vector2 end)
    {
        float elapsed = 0f;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionTime;

            rect.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        rect.anchoredPosition = end;
    }
    #endregion

    private void PopulateData(string achievementID)
    {
        // If there is no last achievement, use placeholder data
        if (achievementID == null)
        {
            SetPlaceholders();
        }

        // Grab the achievement for the for the AchievmentID
        Achievement achievement = AchievementManager.Instance.GetAchievement(achievementID);

        // If the achievement doesn't exist, uce placeholder data
        if (achievement == null)
        {
            SetPlaceholders();
        }

        // If achivement.name is null or empty, use placeholder text
        if (string.IsNullOrEmpty(achievement.name))
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
            rewardText.text = "Reward: " + achievement.reward;
        }
    }

    private void SetPlaceholders()
    {
        nameText.text = namePlaceholder;
        flavorText.text = flavorPlaceholder;
        descriptionText.text = descriptionPlaceholder;
        rewardText.text = rewardPlaceholder;
    }

    // Event listener which will call for the pop-up every time an achievemnet is earned
    private void OnEnable()
    {
        GameEvent.OnAchivementEarned += DisplayPopUp;
    }

    private void OnDisable()
    {
        GameEvent.OnAchivementEarned -= DisplayPopUp;
    }
}
