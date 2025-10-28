using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public int PTOAmount; //Total amount of currency the player has (Do we need to do more than this right now?)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // RL: Method which will try to spend the player's PTO. Returns False if the player lacks the necessary funds
    public bool SpendPTO(int spentPTO)
    {
        // If spending the PTO would put the player into the negatives
        if (PTOAmount - spentPTO < 0)
        {
            // Return False and leave PTO alone
            return false;
        }
        // If the player has enough PTO
        else
        {
            // Deduct the Spent PTO from the player's total
            PTOAmount -= spentPTO;
            // Return true
            return true;
        }
        
    }
}
