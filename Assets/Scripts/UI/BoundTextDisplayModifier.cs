using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoundTextDisplayModifier : MonoBehaviour
{
    private InputManager InputManager;

    [SerializeField] private ActionType selectedAction;
    [SerializeField] private bool useCustomText = false;
    [SerializeField] private string customText = string.Empty; //The half to be to shown before interjected text
    [SerializeField] private string customText2 = string.Empty; //The half to be shown after the inerjected text
    private TextMeshPro text;
    //This index allows for situational things like the "movement" to be shown with index for the future
    // [SerializeField] private int hardCodedIndex = 0;
    // [SerializeField] private int controllerIndexBump = 4;
    // [SerializeField] private bool useCustomIndex = false;

    #region Action Type Mapping
    public enum ActionType
    {
        Move,
        Fire,
        AltFire,
        Jump,
        Scroll,
        Look,
        Sprint,
        Reload,
        Next,
        Interact,
        Drop,
        Cheats,
        Controller,
        Keyboard,
        ControllerBack,
        MidGameUIView
    }

    public InputAction GetAction(ActionType actionType)
    {
        return actionType switch
        {
            ActionType.Move => InputManager.moveAction,
            ActionType.Fire => InputManager.fireAction,
            ActionType.AltFire => InputManager.altFireAction,
            ActionType.Jump => InputManager.jumpAction,
            ActionType.Scroll => InputManager.scrollAction,
            ActionType.Look => InputManager.lookAction,
            ActionType.Sprint => InputManager.sprintAction,
            ActionType.Reload => InputManager.reloadAction,
            ActionType.Next => InputManager.nextAction,
            ActionType.Interact => InputManager.interactAction,
            ActionType.Drop => InputManager.dropAction,
            ActionType.Cheats => InputManager.cheatsAction,
            ActionType.Controller => InputManager.controllerAction,
            ActionType.Keyboard => InputManager.keyboardAction,
            ActionType.ControllerBack => InputManager.controllerBackAction,
            ActionType.MidGameUIView => InputManager.MidGameUIViewAction,
            _ => null
        };
    }

    #endregion


    private void Start()
    {
        InputManager = GameObject.FindAnyObjectByType<InputManager>();
        text = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (InputManager != null && useCustomText && text != null)
        {
            text.text = customText + ReturnRelevantText() + customText2;
        }
        else
        {
            Debug.Log("Input Manager is null: " + (InputManager == null) + "\nuseCustomText is: " + useCustomText + "\ntext is null: " + (text == null));
            if(InputManager == null) InputManager = GameObject.FindAnyObjectByType<InputManager>();
            if(text == null) text = GetComponent<TextMeshPro>();
        }
    }

    public string ReturnRelevantText()
    {
        string ret;

        if (InputManager != null)
        {
            int index = 0;
            if (InputManager.ControllerLast) index = 1;
            ret = GetAction(selectedAction).GetBindingDisplayString(index);
            return ret;
        }
        else
        {
            InputManager = GameObject.FindAnyObjectByType<InputManager>();
            return "Bind Action";
        }
    }

}
