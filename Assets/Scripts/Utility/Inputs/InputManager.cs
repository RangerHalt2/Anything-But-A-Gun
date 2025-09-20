//Purpose: To handle all inputs in a dynamic system that can be updated in future iterations of the game and add new inputs easily.
//Contributor and Author: Logan Baysinger. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerControls;

    [SerializeField] private string actionMapName = "Player";//This should be the name of the entire mapping

    //LB: These should be the names of individual input readings for the broad inputs like WASD is part of movement and jumping assigns spacebar and A/Circle on controllers
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string fire = "Fire";
    [SerializeField] private string altFire = "AltFire";
    [SerializeField] private string jump = "Jump";

    //LB: This is an action input, each one needs one assigned
    private InputAction moveAction;
    private InputAction fireAction;
    private InputAction altFireAction;
    private InputAction jumpAction;

    //LB: This is the getters and setters for the inputs, this will be used to manage their values overall
    public Vector2 MoveInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool AltFireInput { get; private set; }
    public bool JumpInput { get; private set; }

    //LB: Instance Handler
    public static InputManager Instance { get; private set; }

    void Start()
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
    }

    //LB: Enable and Disable the actions
    private void OnEnable()
    {
        moveAction.Enable();
        fireAction.Enable();
        altFireAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        fireAction.Disable();
        altFireAction.Disable();
        jumpAction.Disable();
    }
}
