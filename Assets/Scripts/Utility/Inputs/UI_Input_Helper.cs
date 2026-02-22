using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class UI_Input_Helper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UP_Keybind;
    [SerializeField] private TextMeshProUGUI DOWN_Keybind;
    [SerializeField] private TextMeshProUGUI LEFT_KeyBind;
    [SerializeField] private TextMeshProUGUI RIGHT_KeyBind;

    [SerializeField] private TextMeshProUGUI Fire_Keybind;
    [SerializeField] private TextMeshProUGUI Reload_Keybind;
    [SerializeField] private TextMeshProUGUI Jump_Keybind;
    [SerializeField] private TextMeshProUGUI Dash_Keybind;
    [SerializeField] private TextMeshProUGUI Interact_Keybind;
    [Space(2)]
    [SerializeField] private Canvas KeyboardCanvas;
    [SerializeField] private Canvas ControllerCanvas;
    [Space(2)]
    [SerializeField] private TextMeshProUGUI CNTRL_UP_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_DOWN_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_LEFT_KeyBind;
    [SerializeField] private TextMeshProUGUI CNTRL_RIGHT_KeyBind;

    [SerializeField] private TextMeshProUGUI CNTRL_Fire_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_Reload_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_Jump_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_Dash_Keybind;
    [SerializeField] private TextMeshProUGUI CNTRL_Interact_Keybind;

    private InputManager inputManager;

    private bool isKeyboard = true;

    private void Start()
    {
        inputManager = GameObject.FindAnyObjectByType<InputManager>();
    }

    private void Update()
    {
        if (inputManager != null)
        {
            UP_Keybind.text = inputManager.moveAction.GetBindingDisplayString(1);
            DOWN_Keybind.text = inputManager.moveAction.GetBindingDisplayString(2);
            LEFT_KeyBind.text = inputManager.moveAction.GetBindingDisplayString(3);
            RIGHT_KeyBind.text = inputManager.moveAction.GetBindingDisplayString(4);

            Fire_Keybind.text = inputManager.fireAction.GetBindingDisplayString(0);
            Reload_Keybind.text = inputManager.reloadAction.GetBindingDisplayString(0);
            Jump_Keybind.text = inputManager.jumpAction.GetBindingDisplayString(0);
            Dash_Keybind.text = inputManager.sprintAction.GetBindingDisplayString(0);
            Interact_Keybind.text = inputManager.interactAction.GetBindingDisplayString(0);

            CNTRL_UP_Keybind.text = inputManager.moveAction.GetBindingDisplayString(6);
            CNTRL_DOWN_Keybind.text = inputManager.moveAction.GetBindingDisplayString(7);
            CNTRL_LEFT_KeyBind.text = inputManager.moveAction.GetBindingDisplayString(8);
            CNTRL_RIGHT_KeyBind.text = inputManager.moveAction.GetBindingDisplayString(9);

            CNTRL_Fire_Keybind.text = inputManager.fireAction.GetBindingDisplayString(1);
            CNTRL_Reload_Keybind.text = inputManager.reloadAction.GetBindingDisplayString(1);
            CNTRL_Jump_Keybind.text = inputManager.jumpAction.GetBindingDisplayString(1);
            CNTRL_Dash_Keybind.text = inputManager.sprintAction.GetBindingDisplayString(1);
            CNTRL_Interact_Keybind.text = inputManager.interactAction.GetBindingDisplayString(1);

        }
    }

    public void UIRebindCall(Rebindable_ScriptableObject actionData)
    {

        if (inputManager != null)
        {
            inputManager.RebindInput(actionData.action, actionData.bindingIndex);
        }
    }


    public void ControllerKeyboardToggle(bool keyboard)
    {
        if (keyboard)
        {
            KeyboardCanvas.gameObject.SetActive(true);
            ControllerCanvas.gameObject.SetActive(false);
        }
        else
        {
            KeyboardCanvas.gameObject.SetActive(false);
            ControllerCanvas.gameObject.SetActive(true);
        }
    }



}
