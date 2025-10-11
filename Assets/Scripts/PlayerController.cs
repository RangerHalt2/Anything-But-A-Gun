using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController characterController;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;

    [SerializeField] private float sensitivity = 10f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -30f;

    private WeaponHandler weaponHandler;
    private InputManager inputs;

    private Health playerHealth;

    private WinEvent winEvent;

    private float rotationY;
    private float verticalForce;
    
    [Space(5)]
    [Header("Dash Variables")]
    [SerializeField] private float dashForce; //How strong the dash is
    [SerializeField] private float dashTime; //The time the player spends dashing
    [SerializeField] private int maxDashLimit = 3; //The number of times that the player can dash
    [SerializeField] private float currentDashCharges; //The number of dashes the player currently has
    [SerializeField] private float dashCharge = 0f; //How much charge the player has to get a new dash
    [SerializeField] private float dashChargeTime = 5f; //How long the player needs to get a new dash
    [SerializeField] private float dashCd = 0.25f; //Don't immediately dash again after the first dash
    private bool canDash = true;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //This might better belong on a different script? Unsure
        playerHealth = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        weaponHandler = GetComponent<WeaponHandler>();
        winEvent = GameObject.FindAnyObjectByType<WinEvent>();

        currentDashCharges = maxDashLimit;
    }

    private void Move(Vector2 MovementVector)
    {
        //Default Base Case
        Vector3 move = transform.forward * MovementVector.y + transform.right * MovementVector.x;
        move = movementSpeed /** (inputs.SprintInput? sprintMultiplier : 1)*/ * Time.deltaTime * move;
        characterController.Move(move);

        verticalForce = verticalForce + gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, verticalForce, 0) * Time.deltaTime);
    }

    private void Rotate(Vector2 RotationVector)
    {
        rotationY += RotationVector.x * sensitivity;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }

    private void StartDash()
    {
        if (canDash && currentDashCharges > 0)
        {
            StartCoroutine(Dash());
        }
    }
    

    private IEnumerator Dash()
    {
        currentDashCharges--;
        canDash = false;
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward;

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * movementSpeed * dashForce * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(dashCd);
        canDash = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.isDead || winEvent.hasWon) return;
        Move(inputs.MoveInput);
        Rotate(inputs.LookInput);

        if(inputs.JumpInput == true && characterController.isGrounded)
        {
            verticalForce = jumpForce; //The Jump itself is handled in the Move() method handling gravity, making use of CharacterController instead of RigidBody
        }

        if (inputs.FireInput)
        {
            weaponHandler.FireWeapon();
        }
        if (inputs.ReloadInput) 
        { 
            weaponHandler.ReloadWeapon();
        }
        if (inputs.SprintInput)
        {
            StartDash();
        }

        //Dash variable regulation
        if (currentDashCharges < maxDashLimit)
        {
            dashCharge += Time.deltaTime; //This one says if you don't have the max dash charges, build up to a new one
        }
        if (dashCharge >= dashChargeTime)
        { 
            dashCharge = 0;
            currentDashCharges++; //This one says if you have enough charge for a new dash, store the new dash
        }


        TimerDecrement();
    }

    //LB: Any and all future timers for the player will be managed in this area
    private void TimerDecrement()
    {
        
    }

}
