using UnityEngine;
using UnityEngine.Rendering;

public class StyleGaugeController : MonoBehaviour
{
    public GameObject[] letter; //C, B, A, S, etc.
    public float[] color; //How vibrant the scene is. Based on level.
    private int maxLevel;
    private Volume bWBoxVolume;
    [SerializeField] float score; //The score the player currently holds. Pretty self explanatory.
    private int level; //What letter should be active;
    [SerializeField] float difBetweenLevels; //The score difference required to get to the next level.
    [SerializeField] float scoreStallTimer; //Time that the timer stalls for before the score starts decreasing.
    [SerializeField] float scoreDecreaseSpeed = 1f; //Modifies how fast the score decreases.
    [SerializeField] float critHitScoreAdd; //Amount added if the player scores a crit.
    [SerializeField] float enemyDeathScoreAdd; //Amount added if the player kills an enemy. (Later we can make this different for each enemy if we want)
    [SerializeField] float playerDamageScoreSubtract; //Amount subtracted when the player takes damage.
    private float stall; //This is the timer that will count up to the scoreStallTimer value.

    void Start() 
    {
        bWBoxVolume = GameObject.Find("BW Box Volume").GetComponent<Volume>();
        maxLevel = letter.Length - 1;
        bWBoxVolume.blendDistance = color[level];
    }

    // Update is called once per frame
    void Update()
    {
        if (stall > 0) //Stalls the score decrease
        {
            stall -= Time.deltaTime;
        }
        else if (score > 0) //Decrease the score overtime
        {
            score -= Time.deltaTime * scoreDecreaseSpeed;
        }

        if (score <= 0 && level > 0) //Level down
        {
            score += difBetweenLevels;
            if (letter[level] != null) { letter[level].SetActive(false); }
            level--;
            Debug.Log("Level is now: " + level);
            bWBoxVolume.blendDistance = color[level];
            if (letter[level] != null) { letter[level].SetActive(true); }
        }
        else if (score <= 0 && level == 0) 
        {
            score = 0;
        }

        if (score > difBetweenLevels && level < maxLevel) //Increase your level and change the letter (This will also be where filters are applied)
        {
            score -= difBetweenLevels;
            if (letter[level] != null) { letter[level].SetActive(false); }
            level++;
            bWBoxVolume.blendDistance = color[level];
            if (letter[level] != null) { letter[level].SetActive(true); }
        }
        else if (score > difBetweenLevels && level == maxLevel) 
        { 
            score = difBetweenLevels;
        }
    }

    public void IncreaseScore(bool enemyDeath, bool critHit) //When the player kills an enemy, or crits.
    {
        stall = scoreStallTimer;

        if (enemyDeath)
        {
            score += enemyDeathScoreAdd;
        }
        if (critHit)
        {
            score += critHitScoreAdd;
        }
    }

    public void DecreaseScore() //When the player get's hit
    {
        score -= playerDamageScoreSubtract;
    }
}
