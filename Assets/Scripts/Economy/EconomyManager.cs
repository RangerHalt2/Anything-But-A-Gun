using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    public int PTOAmount; //Total amount of currency the player has (Do we need to do more than this right now?)

    [SerializeField] private bool infinitePTOMode = false; // When set to true, the player will never spend PTO 

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

    private void OnEnable()
    {
        GameEvent.gainPTO += HandleLoadsemone;
    }

    private void HandleLoadsemone()
    {
        if (PTOAmount >= 365)
        {
            AchievementManager.Instance.UnlockAchievement("loadsemone");
        }
    }

    // RL: Method which will try to spend the player's PTO. Returns False if the player lacks the necessary funds
    public bool SpendPTO(int spentPTO)
    {
        // Check if infinite PTO Mode has been enabled
        if (infinitePTOMode)
        {
            // If infinite PTO mode is enabled, then pass the spending check without effecting the player's PTO
            return true;
        }

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
            GameEvent.spendPTO?.Invoke();
            // Return true
            return true;
        }
    }

    public void ToggleInfinitePTO()
    {
        if (infinitePTOMode)
        {
            infinitePTOMode = false;
            return;
        }
        else
        {
            infinitePTOMode = true;
            return;
        }
    }
}
