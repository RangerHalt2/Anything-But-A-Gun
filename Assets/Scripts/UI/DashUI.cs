using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    PlayerController player;

    private GameObject[] dashContainers;
    private Image[] dashFills;
    public Transform dashesParent;
    public GameObject dashContainerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
        dashContainers = new GameObject[PlayerController.Instance.maxDashLimit];
        dashFills = new Image[PlayerController.Instance.maxDashLimit];

        PlayerController.Instance.onDashChangedCallback += UpdateDashesHUD;
        InstantiateDashContainers();
        UpdateDashesHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void SetDashContainers()
    {
        for (int i = 0; i < dashContainers.Length; i++)
        {
            if (i < PlayerController.Instance.maxDashLimit)
            {
                dashContainers[i].SetActive(true);
            }
            else
            {
                dashContainers[i].SetActive(false);
            }
        }
    }

    void SetFilledDashes()
    {
        for (int i = 0; i < dashFills.Length; i++)
        {
            if (i < PlayerController.Instance.Dashes)
            {
                dashFills[i].fillAmount = 1;
            }
            else
            {
                dashFills[i].fillAmount = 0;
            }
        }
    }

    public void InstantiateDashContainers()
    {
        if (player == null) player = PlayerController.Instance;

        if (dashContainers == null || dashContainers.Length != player.maxDashLimit)
        {
            dashContainers = new GameObject[player.maxDashLimit];
            dashFills = new Image[player.maxDashLimit];
        }

        // RL: Destroy existing containers prior to instantiation
        foreach (Transform child in dashesParent)
        {
            Destroy(child.gameObject);
        }

        int dashCount = player.maxDashLimit;

        dashContainers = new GameObject[dashCount];
        dashFills = new Image[dashCount];

        for (int i = 0; i < dashCount; i++)
        {
            GameObject temp = Instantiate(dashContainerPrefab);
            temp.transform.SetParent(dashesParent, false);
            dashContainers[i] = temp;
            dashFills[i] = temp.transform.Find("dashFill").GetComponent<Image>();
        }

        UpdateDashesHUD();
    }

    void UpdateDashesHUD()
    {
        if (dashFills == null) return;
        SetDashContainers();
        SetFilledDashes();
    }
    
}
