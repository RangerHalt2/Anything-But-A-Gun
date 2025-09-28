using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController characterController;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float sprintMulti = 2;

    [SerializeField] private float sensitivity = 10f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -30f;

    private WeaponHandler weaponHandler;
    private InputManager inputs;

    private float rotationY;
    private float verticalForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //This might better belong on a different script? Unsure
        characterController = GetComponentInChildren<CharacterController>();
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        weaponHandler = GetComponent<WeaponHandler>();
    }

    private void Move(Vector2 MovementVector)
    {
        //Default Base Case
        Vector3 move = transform.forward * MovementVector.y + transform.right * MovementVector.x;
        move = movementSpeed * (inputs.SprintInput ? sprintMulti : 1 ) * Time.deltaTime * move;
        characterController.Move(move);

        verticalForce = verticalForce + gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, verticalForce, 0) * Time.deltaTime);
    }

    private void Rotate(Vector2 RotationVector)
    {
        rotationY += RotationVector.x * sensitivity * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }


    // Update is called once per frame
    void Update()
    {
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

        TimerDecrement();
    }

    //LB: Any and all future timers for the player will be managed in this area
    private void TimerDecrement()
    {
        
    }

}
