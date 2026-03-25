using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// This script was written by Aaron and is used with the "Game Over" UI element

public class EndingGrade : MonoBehaviour
{
    // I will need this script to access the meta progression variables. Please do the code magic to make that happen thx
    public int totalEnemiesKilled;
    public int endingScore;
    public string endingScoreString = "abcdef";
    public string endingScoreStringTyped;
    public GameObject endingScoreObject;
    public TextMeshProUGUI endingScoreText;

    public string yourScore = "Your Score:";
    public string yourScoreTyped = "";
    public GameObject yourScoreObject;
    public TextMeshProUGUI yourScoreText;

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
        buttonGroupObject.SetActive(false); // set this to false!!!
    }

    public IEnumerator EndingGradeCoroutine(){ // Run this coroutine when the player gets game over

        Debug.Log("ENDING COROUTINE STARTED");

        // step 1: count how many enemies were killed AND what level the player is currently on
            // 1.1: do some sort of math to get a score value (i.e. (enemiesKilled x 100) + (levelsCompleted x 2000))
        // step 2: set endingScore to that value and set endingScoreString to that value as well
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
        Debug.Log("look here " + endingScoreString.Length);

        for (int j = 0; j < 6; j++)
        {
            endingScoreStringTyped = endingScoreString[Mathf.Abs(j - 5)] + endingScoreStringTyped;
            endingScoreText.text = endingScoreStringTyped;
            if (textWritingSFX != null)
                Instantiate(textWritingSFX, transform.position, transform.rotation, null);
            yield return new WaitForSecondsRealtime(textWritingInterval);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        letterGradeObject.SetActive(true);
        if (textWritingSFX != null)
            Instantiate(letterGradeSFX, transform.position, transform.rotation, null);

        yield return new WaitForSecondsRealtime(0.5f);
        buttonGroupObject.SetActive(true);

        yield break;
    }
}
