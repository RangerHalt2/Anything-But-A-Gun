using System.Collections;
using System.Collections.Generic;
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

    void InstantiateDashContainers()
    {
        for (int i = 0; i < PlayerController.Instance.maxDashLimit; i++)
        {
            GameObject temp = Instantiate(dashContainerPrefab);
            temp.transform.SetParent(dashesParent, false);
            dashContainers[i] = temp;
            dashFills[i] = temp.transform.Find("dashFill").GetComponent<Image>();
        }
    }

    void UpdateDashesHUD()
    {
        SetDashContainers();
        SetFilledDashes();
    }
}
