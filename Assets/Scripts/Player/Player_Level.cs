//Author and Contributor: Logan Baysinger
//Purpose: Handles the level of the player and core implementation calculating their level

//TO DO: Add and consider how this level interacts with the bootstrapper, or is not destroyed on the player's load.

using UnityEngine;

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
    #endregion


    //LB: These are getters and setters for accessing the level, the info should not be necessarily displayed in the editor
    [HideInInspector]
    public float EXP { get; private set; } = 0;
    [HideInInspector]
    public int Level { get; private set; } = 0;

    //LB: A public function to be called on the enemies death, adds exp to the player.
    //Rounds solely to prevent fractions of exp. 
    public void AddEXP(float EXP)
    {
        this.EXP += EXP;
        this.EXP = Mathf.Round(this.EXP); 
    }

    #region Calculate Level with Formula
    //LB: The formula is such: (x/A)^(1/P) where A gets to be the floor for lvl 1 and P gets to be the growth rate of the exp to level mark. Speak to Logan for more info.
    private int CalculateLevel()
    {
        int level = this.Level;

        level = (int)(Mathf.Pow((EXP/floor), (1 / levelGrowth)));

        if(level > maxLevel) level = maxLevel;

        this.Level = level;

        return level;
    }
    #endregion

}
