//Purpose: To handle all inputs in a dynamic system that can be updated in future iterations of the game and add new inputs easily.
//Contributor and Author: Logan Baysinger. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] public InputActionAsset playerControls;

    [SerializeField] public string actionMapName = "Player";//This should be the name of the entire mapping

    //LB: These should be the names of individual input readings for the broad inputs like WASD is part of movement and jumping assigns spacebar and A/Circle on controllers
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string altFire = "AltFire";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string mouseScrollY = "MouseScrollY";
    [SerializeField] private string look = "Look";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string reload = "Reload";
    [SerializeField] private string next = "NextWeapon";
    [SerializeField] private string interact = "Interact";

    //LB: This is an action input, each one needs one assigned
    private InputAction moveAction;
    private InputAction fireAction;
    private InputAction altFireAction;
    private InputAction jumpAction;
    public InputAction scrollAction;
    public InputAction lookAction;
    public InputAction sprintAction;
    public InputAction reloadAction;
    private InputAction nextAction;
    private InputAction interactAction;

    //LB: This is the getters and setters for the inputs, this will be used to manage their values overall
    public Vector2 MoveInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool AltFireInput { get; private set; }
    public bool JumpInput { get; private set; }
    public float MouseScrollInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool SprintInput { get; private set; }
    public bool ReloadInput { get; private set; }
    public float NextInput { get; private set; }
    public bool InteractInput { get; private set; }

    //LB: Instance Handler
    public static InputManager Instance { get; private set; }

    void Awake()
    {
        //LB: Handles instance related stuff, still unsure where and when the destroy code runs? It never seems necessary
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
        
        //LB: This assigns the values of the actions to the input actions from the physical asset
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(movement);
        fireAction = playerControls.FindActionMap(actionMapName).FindAction(fire);
        altFireAction = playerControls.FindActionMap(actionMapName).FindAction(altFire);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        scrollAction = playerControls.FindActionMap(actionMapName).FindAction(mouseScrollY);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        reloadAction = playerControls.FindActionMap(actionMapName).FindAction(reload);
        nextAction = playerControls.FindActionMap(actionMapName).FindAction(next);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);
        RegisterInputActions();
    }

    //LB: This piece of code handles their values with pseudo functions
    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        fireAction.performed += context => FireInput = true;
        fireAction.canceled += context => FireInput = false;

        altFireAction.performed += context => AltFireInput = true;
        altFireAction.canceled += context => AltFireInput = false;

        jumpAction.performed += context => JumpInput = true;
        jumpAction.canceled += context => JumpInput = false;

        scrollAction.performed += context => MouseScrollInput = context.ReadValue<float>();
        scrollAction.canceled += context => MouseScrollInput = 0f;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        sprintAction.performed += context => SprintInput = true;
        sprintAction.canceled += context => SprintInput = false;

        reloadAction.performed += context => ReloadInput = true;
        reloadAction.canceled += context => ReloadInput = false;

        nextAction.performed += context => NextInput = context.ReadValue<float>();
        nextAction.canceled += context => NextInput = 0;

        interactAction.performed += context => InteractInput = true;
        interactAction.canceled += context => InteractInput = false;
    }

    //LB: Enable and Disable the actions
    private void OnEnable()
    {
        moveAction.Enable();
        fireAction.Enable();
        altFireAction.Enable();
        jumpAction.Enable();
        scrollAction.Enable();
        lookAction.Enable();
        sprintAction.Enable();
        reloadAction.Enable();
        nextAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        fireAction.Disable();
        altFireAction.Disable();
        jumpAction.Disable();
        scrollAction.Disable();
        lookAction.Disable();
        sprintAction.Disable();
        reloadAction.Disable();
        nextAction.Disable();
        interactAction.Disable();
    }

}
