using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    [SerializeField] private Image scoreFill; //The fill portion of the style UI
    [SerializeField] private GameObject totalGauge; //The full gauge for turning off and on.
    [SerializeField] private GameObject gaugeVFX; //Same thing as last ^^^
    [SerializeField] private float simulationSpeedChange = 1f; //The amount that the simulationSpeed of the above VFX changes
    [SerializeField] private float colorChangeSpeed = 1f; //The speed that the color changes when a player levels up;

    void Start() 
    {
        totalGauge.SetActive(false);
        gaugeVFX.SetActive(false);
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
            if (letter[level] != null) { letter[level].SetActive(true); }
            if (gaugeVFX != null)
            {
                var main = gaugeVFX.GetComponent<ParticleSystem>().main;
                main.simulationSpeed -= simulationSpeedChange;
            }
        }
        else if (score <= 0 && level == 0) 
        {
            score = 0;
            if(totalGauge != null)
                totalGauge.SetActive(false);
            if(gaugeVFX != null)
                gaugeVFX.SetActive(false);
        }

        if (score > difBetweenLevels && level < maxLevel) //Increase your level and change the letter (This will also be where filters are applied)
        {
            score -= difBetweenLevels;
            if (letter[level] != null) { letter[level].SetActive(false); }
            level++;
            
            if (letter[level] != null) { letter[level].SetActive(true); }
            if (gaugeVFX != null) 
            {
                var main = gaugeVFX.GetComponent<ParticleSystem>().main;
                main.simulationSpeed += simulationSpeedChange; 
            }

            // RL: event for achivements
            if (level == maxLevel)
            {
                GameEvent.StyleMaxxed?.Invoke();
            }
        }
        else if (score > difBetweenLevels && level == maxLevel) 
        { 
            score = difBetweenLevels;
        }

        // Set the slider's value to the current percentage of health the object has
        float fillPercent = Mathf.Clamp01(score / difBetweenLevels);
        scoreFill.fillAmount = fillPercent;

        //Blend Distance 
        if (bWBoxVolume.blendDistance < color[level]) { bWBoxVolume.blendDistance += (Time.deltaTime / level) * colorChangeSpeed; }
        else if (bWBoxVolume.blendDistance > color[level]) { bWBoxVolume.blendDistance -= (Time.deltaTime / level) * colorChangeSpeed; ; }
        else { bWBoxVolume.blendDistance = color[level]; }
    }

    public void IncreaseScore(bool enemyDeath, bool critHit) //When the player kills an enemy, or crits.
    {
        if (!totalGauge.activeSelf) 
        {
            if(totalGauge != null)
                totalGauge.SetActive(true);
            if(gaugeVFX != null)
                gaugeVFX.SetActive(true);
        }

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
