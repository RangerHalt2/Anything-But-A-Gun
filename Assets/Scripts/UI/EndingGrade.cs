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
    public string endingScoreString;
    public string endingScoreStringTyped;
    public GameObject endingScoreObject;
    public TextMeshProUGUI endingScoreText;

    public string yourScore = "Your Score:";
    public string yourScoreTyped = "";
    public GameObject yourScoreObject;
    public TextMeshProUGUI yourScoreText;

    public GameObject letterGradeObject;

    public GameObject buttonGroupObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        yourScoreTyped = "";
        yourScoreObject.SetActive(false);
        endingScoreObject.SetActive(false);
        letterGradeObject.SetActive(false);
        buttonGroupObject.SetActive(true); // set this to false!!!
    }

    public IEnumerator EndingGradeCoroutine(){ // Run this coroutine when the player gets game over

        Debug.Log("ENDING COROUTINE STARTED");

        // step 1: count how many enemies were killed AND what level the player is currently on
            // 1.1: do some sort of math to get a score value (i.e. (enemiesKilled x 100) + (levelsCompleted x 2000))
        // step 2: set endingScore to that value and set endingScoreString to that value as well
        // step 3: enable yourScoreObject to be visible
        yield return new WaitForSeconds(0.5f);
        yourScoreObject.SetActive(true);

        // step 4: use forloop to loop this set of code for every character in the yourScore string, increasing a variable at the same time
            // 4.1: take Nth letter of yourScore string and append it to yourScoreTyped string
            // 4.2: change text shown on screen to yourScoreTyped
            // 4.3: play random keyboard type sound effect from array
        // step 5: wait for X seconds
        yield return new WaitForSeconds(0.5f);
        endingScoreObject.SetActive(true);

        // step 6: new forloop to loop this set of code like last time
            // 6.1: take Nth letter FROM THE OTHER DIRECTION of endingScoreString and append it to endingScoreStringTyped string
            // 6.2: change text shown on screen to endingScoreStringTyped
            // 6.3: play random sfx
        // step 7: wait X seconds
        yield return new WaitForSeconds(0.5f);
        letterGradeObject.SetActive(true);

        // step 8: enable scoreLetterObject
            // 8.1: play sfx
        // step 9: enable all buttons
        yield return new WaitForSeconds(0.5f);
        buttonGroupObject.SetActive(true);

        yield break;
    }
}
