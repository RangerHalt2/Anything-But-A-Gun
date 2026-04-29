using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;

// This script was written by Aaron and is used with the "Game Over" UI element

public class EndingGrade : MonoBehaviour
{
    // I will need this script to access the meta progression variables. Please do the code magic to make that happen thx
    public int totalEnemiesKilled;
    public int endingLevel;
    private int tempScore;
    public int endingScore;
    public string endingScoreString = "abcdef";
    public string endingScoreStringTyped;
    public GameObject endingScoreObject;
    public TextMeshProUGUI endingScoreText;

    public string yourScore = "Your Score:";
    public string yourScoreTyped = "";
    public GameObject yourScoreObject;
    public TextMeshProUGUI yourScoreText;

    public string letterGrade;
    public TextMeshProUGUI letterGradeText;
    public GameObject letterGradeObject;

    public GameObject buttonGroupObject;

    [Tooltip("The # of seconds between each typed out letter of the ending grade cutscene")]
    public float textWritingInterval = 0.1f;
    public GameObject textWritingSFX;
    public GameObject letterGradeSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yourScoreTyped = "";
        endingScoreText.text = "";
        yourScoreObject.SetActive(false);
        endingScoreObject.SetActive(false);
        letterGradeObject.SetActive(false);
        //buttonGroupObject.SetActive(false); // set this to false!!!
    }

    public IEnumerator EndingGradeCoroutine(){ // Run this coroutine when the player gets game over
        yourScoreTyped = "";
        endingScoreText.text = "";
        Debug.Log("ENDING COROUTINE STARTED");
        // RL: Accessing RunData to calculate ending Score

        // Get the total number of enemies killed in the run from the Run Data JSON
        totalEnemiesKilled = RunDataRecorder.Instance.GetEnemiesKilled();
        //Debug.Log("Ending Grade: Enemies killed: " + totalEnemiesKilled + " according to RunData");

        // Get the level the player ended on
        endingLevel = RunDataRecorder.Instance.GetLevelsCompleted() + 1; // Adding one since player score utilizes the level the player ended on
        //Debug.Log("Ending Grade: Levels Completed " + endingLevel + " according to RunData");

        // Calculate Ending Score
        tempScore = (totalEnemiesKilled * 100) + (endingLevel * 2000);

        // Calculate Ending Grade (added by Aaron)
        if (tempScore >= 2000)
        {
            if (tempScore >= 4000)
            {
                if (tempScore >= 6000)
                {
                    if (tempScore >= 8000)
                    {
                        letterGrade = "A";
                    }
                    else
                    {
                        letterGrade = "B";
                    }
                }
                else
                {
                    letterGrade = "C";
                }
            }
            else
            {
                letterGrade = "D";
            }
        }
        else
        {
            letterGrade = "F";
        }

        // Set ending Score to the appropriate Value
        endingScore = tempScore;

        // Set Ending Score String
        endingScoreString = endingScore.ToString();
        //Debug.Log("Ending Grade: Score string " + endingScoreString);


        // step 3: enable yourScoreObject to be visible
        yield return new WaitForSecondsRealtime(0.5f);
        yourScoreObject.SetActive(true);

        for (int i = 0; i < yourScore.Length; i++)
        {
            yourScoreTyped = yourScoreTyped + yourScore[i];
            yourScoreText.text = yourScoreTyped;
            if (textWritingSFX != null)
                Instantiate(textWritingSFX, transform.position, transform.rotation, null);
            yield return new WaitForSecondsRealtime(textWritingInterval);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        endingScoreObject.SetActive(true);

        endingScoreStringTyped = "";

        for (int j = endingScoreString.Length - 1; j >= 0; j--)
        {
            endingScoreStringTyped = endingScoreString[j] + endingScoreStringTyped;
            endingScoreText.text = endingScoreStringTyped;

            if (textWritingSFX != null)
                Instantiate(textWritingSFX, transform.position, transform.rotation, null);

            yield return new WaitForSecondsRealtime(textWritingInterval);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        letterGradeText.text = letterGrade;
        letterGradeObject.SetActive(true);
        if (textWritingSFX != null)
            Instantiate(letterGradeSFX, transform.position, transform.rotation, null);

        yield return new WaitForSecondsRealtime(0.5f);
        //buttonGroupObject.SetActive(true);

        yield break;
    }
}
