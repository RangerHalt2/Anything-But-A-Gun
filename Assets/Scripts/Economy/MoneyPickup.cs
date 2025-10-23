using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    [SerializeField] private EconomyManager ecoManager;
    [SerializeField] private int amount;

    void Start()
    {
        ecoManager = EconomyManager.Instance;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider _other) 
    { 
        if (_other.CompareTag("Player")) 
        {
            ecoManager.PTOAmount += amount;
            Destroy(gameObject);
        }
    }
}
