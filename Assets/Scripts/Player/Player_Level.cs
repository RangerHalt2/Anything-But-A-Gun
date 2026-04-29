//Author and Contributor: Logan Baysinger
//Purpose: Handles the level of the player and core implementation calculating their level

//TO DO: Add and consider how this level interacts with the bootstrapper, or is not destroyed on the player's load.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Level : MonoBehaviour
{
    #region Serialized Variables
    [Tooltip("The max level the player can achieve")]
    [SerializeField] private int maxLevel = 10;

    //Mathematically this is more complex than that, but this is all a Tech needs to know
    [Tooltip("How much EXP is needed for the first level")]
    [SerializeField] private float floor = 1039f;

    //This number CAN go above 2, but the exp required to level up starts to really ramp much faster, the tech team will definitely have to consult us to go above 2
    [Tooltip("How much the level should grow by, this number should range somewhere between 1 and 2")]
    [Range(1f, 2f)]
    [SerializeField] private float levelGrowth = 1.475f;

    private Slider levelBar;
    private TextMeshProUGUI levelNumber;
    #endregion


    //LB: These are getters and setters for accessing the level, the info should not be necessarily displayed in the editor
    //[HideInInspector]
    public float EXP { get; set; } = 0;
    //[HideInInspector]
    public int Level { get; set; } = 1;

    private void Start()
    {
        AddEXP(1);
    }

    //LB: A public function to be called on the enemies death, adds exp to the player.
    //Rounds solely to prevent fractions of exp. 
    public void AddEXP(float EXP)
    {
        Debug.Log($"PLAYER LEVEL - Adding EXP {EXP}");
        this.EXP += EXP;
        this.EXP = Mathf.Round(this.EXP);
        CalculateLevel();
    }

    private void AttachLevelBar()
    {
        PlayerLevelIndicator levelBar = GameObject.FindAnyObjectByType<PlayerLevelIndicator>();
        if (levelBar == null)
        {
            Debug.LogError("PLAYER LEVEL - The levelBar cannot be found");
            return;
        }
        this.levelBar = levelBar.GetComponent<Slider>();

        PlayerLevelNumberIndicator levelNum = GameObject.FindAnyObjectByType<PlayerLevelNumberIndicator>();
        if (levelNum != null)
            levelNumber = levelNum.GetComponent<TextMeshProUGUI>();
    }

    private void UpdateDisplay()
    {
        AttachLevelBar();

        if(Level == maxLevel)
        {
            levelBar.value = 1f; //Fill Amount = 1f;
        }

        if(levelNumber != null)
            levelNumber.text = (Level+1)+"";

        float expCurrentLevel = EXPForLevel(Level);
        float expNextLevel = EXPForLevel(Level + 1);

        Debug.Log($"PLAYER LEVEL - Current EXP: {expCurrentLevel} and Next Level EXP: {expNextLevel}");

        float sliderValue = (EXP - expCurrentLevel) / (expNextLevel - expCurrentLevel);
        Debug.Log($"PLAYER LEVEL - Slider Value: {sliderValue} and setting the value.");
        levelBar.value = Mathf.Clamp01(sliderValue);
    }

    #region Calculate Level with Formula
    //LB: The formula is such: (x/A)^(1/P) where A gets to be the floor for lvl 1 and P gets to be the growth rate of the exp to level mark. Speak to Logan for more info.
    private int CalculateLevel()
    {
        int level = this.Level;

        level = (int)(Mathf.Pow((EXP/floor), (1 / levelGrowth)));

        if(level > maxLevel) level = maxLevel;

        this.Level = level;

        UpdateDisplay();

        return level;
    }

    private float EXPForLevel(int level)
    {
        return floor * Mathf.Pow(level, levelGrowth);
    }

    #endregion

}
