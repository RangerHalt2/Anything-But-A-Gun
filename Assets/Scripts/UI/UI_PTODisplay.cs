using TMPro;
using UnityEngine;

public class UI_PTODisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private EconomyManager m_EconomyManager;


    private void Start()
    {
        m_EconomyManager = EconomyManager.Instance;
        if (m_EconomyManager == null)
        {
            Debug.LogError("UI_PTODisplay - Economy is null turning off");
            this.enabled = false;
        }
    }

    private void Update()
    {

        EconomyManager eco = GameObject.FindAnyObjectByType<EconomyManager>();

        moneyText.text = eco.PTOAmount.ToString();
    }
}
