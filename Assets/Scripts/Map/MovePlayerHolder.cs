using System.Collections;
using TMPro;
using UnityEngine;

public class MovePlayerHolder : MonoBehaviour
{
    private Transform playerHolder;

    [SerializeField] private bool resetHealth;
    [SerializeField] private bool killStyleMeter;

    void Start()
    {
        Debug.LogWarning("Code Started; Attempting to Find Player Holder");
        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if (controller != null)
        {   
            Debug.LogWarning("Player Holder FOUND");
            StartCoroutine(LateStart(controller));
        }
        else
        {
            Debug.LogWarning("Player Holder NOT FOUND");
        }
    }

    private IEnumerator LateStart(PlayerController controller)
    {
        yield return new WaitForSeconds(0.2f);

        playerHolder = controller.transform;

        CharacterController cc = controller.GetComponent<CharacterController>();
        if (cc == null)
        {
            Debug.LogWarning("The character controller could not be found");
            yield return null;
        }
        cc.enabled = false;
        playerHolder.position = transform.position;
        cc.enabled = true;
        controller.isSpawned = true;

        PlayerController pc = cc.GetComponent<PlayerController>();
        if (pc != null && pc.interactionText == null)
        {
            InteractionTextIndicator indicator = GameObject.FindAnyObjectByType<InteractionTextIndicator>(FindObjectsInactive.Include);
            if (indicator != null)
                pc.interactionText = indicator.GetComponent<TextMeshProUGUI>();
        }

        if (GameObject.FindAnyObjectByType<LoadingIndicator>() != null)
        {
            GameObject loadingScreen = GameObject.FindAnyObjectByType<LoadingIndicator>().gameObject;
            loadingScreen.SetActive(false);
            Debug.Log("MOVE PLAYER HOLDER - turning off the loading screen");
        }

        UIManager uiManager = GameObject.FindAnyObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.enabled = false; //Refreshes it's subscriptions
            uiManager.enabled = true;
            uiManager.GoToPage(0);
            uiManager.isPaused = false;
            uiManager.allowPause = true;


            DashUI[] dashUIs = GameObject.FindObjectsByType<DashUI>(FindObjectsSortMode.None);
            foreach (DashUI dashU in dashUIs)
            {
                dashU.player = PlayerController.Instance;
                dashU.enabled = false;
                dashU.enabled = true;
            }
        }

        

        if (killStyleMeter)
        {
            StyleGaugeController sc = GameObject.FindAnyObjectByType<StyleGaugeController>();
            if(sc != null)
            {
                sc.Initialize();
                Debug.Log("MOVE PLAYER HOLDER - Killed the Style Meter");
            }
        }

        if (resetHealth)
        {
            Health playerHealth = cc.GetComponentInChildren<Health>();
            if (playerHealth == null)
            {
                playerHealth = cc.GetComponentInParent<Health>();
            }

            HitIndicatorManager him = GameObject.FindAnyObjectByType<HitIndicatorManager>();
            if (him != null && !playerHealth.HasDamageListeners())
            {
                him.playerHealth = playerHealth;
                him.AssignTookDamageEvent();
            }

            if (GameObject.FindAnyObjectByType<HealthBarIndicator>() != null)
                playerHealth.healthBar = GameObject.FindAnyObjectByType<HealthBarIndicator>().GetComponent<HealthBar>();

            if (GameObject.FindAnyObjectByType<StyleGaugeController>() != null)
                playerHealth.inGameCanvas = GameObject.FindAnyObjectByType<StyleGaugeController>().gameObject;

            playerHealth.currentHealth = playerHealth.maxHealth;
            playerHealth.updateDisplay();
            Debug.Log("MOVE PLAYER HOLDER - Reset the Health");
        }

        WeaponHandler wh = GameObject.FindAnyObjectByType<WeaponHandler>();
        if (wh != null)
        {
            AmmoManager am = wh.currentWeapon.GetComponent<AmmoManager>();
            if (am != null)
            {
                am.updateDisplay();
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}