using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    private CharacterController characterController;

    public CharacterController GetPlayerCharacterController()
    {
        return characterController;
    }

    [SerializeField] private Transform headPoint;

    [Header("Base Movement Fields")]
    public float movementSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;

    [SerializeField] public float sensitivity = 10f;

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float terminalVelocity = -60f;

    [SerializeField] private float controllerSens = 3f;

    private float momentumDecay = 0.0001f;
    private float maxMomentum = 0.03f;

    private WeaponHandler weaponHandler;
    private InputManager inputs;

    private Health playerHealth;

    private WinEvent winEvent;

    private UIManager uiManager;

    private float rotationY;
    private float verticalForce = 0;

    public static PlayerController Instance;

    [Space(5)]
    [Header("Dash Variables")]
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] private float dashForce; //How strong the dash is
    [SerializeField] private float dashTime; //The time the player spends dashing
    [SerializeField] public int maxDashLimit = 3; //The number of times that the player can dash
    [SerializeField] private int dashes; //The number of dashes the player currently has
    [SerializeField] private float dashCharge = 0f; //How much charge the player has to get a new dash
    [SerializeField] private float dashChargeTime = 5f; //How long the player needs to get a new dash
    [SerializeField] private float dashCd = 0.25f; //Don't immediately dash again after the first dash
    public delegate void OnDashChangedDelegate();
    [HideInInspector] public OnDashChangedDelegate onDashChangedCallback;
    public bool canDash = true;
    [SerializeField] private AudioClip dashSFX;

    private Vector3 momentum;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactCooldown = 0.5f;
    [SerializeField] private TextMeshProUGUI interactionText;
    private bool canInteract = true;

    public bool isSpawned = false;

    [Header("Achievement Ability Settings")]
    [SerializeField] private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;
    [SerializeField] private int dashCooldownReduction = 0;
    private float baseDashChargeTime;
    [SerializeField] private int bonusDashes = 0;
    private int baseDashCount;
    #endregion

    #region Getters/Setters
    public CharacterController GetCharacterController() { return characterController; }
    //Jitters the player down miniscually to confirm their grounded state.
    public void JitterDown()
    {
        characterController.Move(new Vector3(0f, -0.05f, 0f));
    }
    #endregion

    [SerializeField] private GameObject dashSound;
    void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSpawned = true;
        Cursor.lockState = CursorLockMode.Locked; //This might better belong on a different script? Unsure
        playerHealth = GetComponent<Health>();
        characterController = GetComponent<CharacterController>();
        inputs = GameObject.FindAnyObjectByType<InputManager>();
        weaponHandler = GetComponent<WeaponHandler>();
        winEvent = GameObject.FindAnyObjectByType<WinEvent>();

        if(dashEffect != null) dashEffect.Stop();

        Dashes = maxDashLimit;

        if (terminalVelocity > 0) terminalVelocity = -terminalVelocity; //Just makes it negative

        sensitivity = LoadSensitivity();

        // RL: Updating player values based on MetaProgression
        // Storing base values for variables that will be effected by Metaprogression
        baseDashChargeTime = dashChargeTime;
        baseDashCount = maxDashLimit;

        hasDoubleJumped = false;
        ApplyAchievementRewards();
    }

    private float LoadSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity", 0.1f);
        // 1.0f is default if no value exists
    }
    #region Movement Handling
    private void Move(Vector2 MovementVector)
    {
        //Default Base Case
        Vector3 move = transform.forward * MovementVector.y + transform.right * MovementVector.x;
        move = movementSpeed /** (inputs.SprintInput? sprintMultiplier : 1)*/ * Time.deltaTime * move;

        //Debug.Log("Move: " + move);

        //LB: If the player stopped moving manually move them incrementally less for a little
        if (!characterController.isGrounded)
        {
            if (move.x == 0f) move.x = momentum.x;
            if (move.z == 0f) move.z = momentum.z;
        }

        verticalForce = verticalForce + gravity * Time.deltaTime;
        verticalForce = Mathf.Clamp(verticalForce, terminalVelocity, -terminalVelocity);
    
        move.y = verticalForce * Time.deltaTime;

        characterController.Move(move);
        if (characterController.isGrounded){ 
            verticalForce = -0.05f; //Vertical Force must always be slightly negative?
            momentum = Vector3.zero;
            hasDoubleJumped = false;
        }
        if (!characterController.isGrounded) {
            momentum = new Vector3(( Mathf.Abs(move.x) - momentumDecay < 0f) ? 0f : move.x - (move.x < 0f ? -momentumDecay : momentumDecay),
                0,
                (Mathf.Abs(move.z) - momentumDecay < 0f) ? 0f : move.z - (move.z < 0f ? -momentumDecay : momentumDecay));
            if(momentum.x > maxMomentum) momentum.x = maxMomentum;
            if(momentum.z > maxMomentum) momentum.z = maxMomentum;
        }

        //Debug.Log("Momentum: " + momentum);
        //Debug.Log("Vertical Force: " + verticalForce);
    }

    private void Rotate(Vector2 RotationVector)
    {
        rotationY += RotationVector.x * sensitivity * (inputs.ControllerLast ? controllerSens : 1f);
        transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }

    private void StartDash()
    {
        if (canDash && dashes > 0)
        {
            if(dashEffect != null) dashEffect.Play();
            StartCoroutine(Dash());
        }
    }
    

    private IEnumerator Dash()
    {
        Dashes--;
        canDash = false;
        float startTime = Time.time;
        Vector3 dashDirection = transform.forward * inputs.MoveInput.y + transform.right * inputs.MoveInput.x;
        if (dashDirection.sqrMagnitude < 0.1 * 0.1f) 
        {
            dashDirection = transform.forward;
        }

        //EW: Need a null check to make sure the dash doesn't fail
        if (dashSound != null)
        {
            // Play sound effect (added by Aaron)
            Instantiate(dashSound, transform.position, transform.rotation, null);
        }

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * movementSpeed * dashForce * Time.deltaTime);
            yield return null;
        }
        if (dashEffect != null) dashEffect.Stop();
        yield return new WaitForSeconds(dashCd);
        canDash = true;
    }

    public int Dashes
    {
        get { return dashes; }
        set
        {
            if (dashes != value)
            {
                dashes = Mathf.Clamp(value, 0, maxDashLimit);

                if (onDashChangedCallback != null)
                {
                    onDashChangedCallback.Invoke();
                }
            }
        }
    }

    #endregion
    // Update is called once per frame     UPDATED BY JAIME AND RYAN 11/3/25
    void Update()
    {
        if (playerHealth.isDead || !isSpawned) return;

        if (winEvent != null  && winEvent.hasWon) return;

        if (UIManager.instance != null && UIManager.instance.IsPaused) return;

        CheckHeadBump();
        
        if(!inputs.JumpPressed() && characterController.isGrounded)
        {
            verticalForce = -2f;
        }

        if (inputs.JumpPressed())
        {
            // Normal Jump
            if (characterController.isGrounded)
            {
                //Debug.Log("Attempting Jump");
                verticalForce = jumpForce; //The Jump itself is handled in the Move() method handling gravity, making use of CharacterController instead of RigidBody
            }
            // Double Jump
            else if (canDoubleJump && !hasDoubleJumped)
            {
                Debug.Log("Player Controller: Attempting Double Jump");
                verticalForce = jumpForce * 0.85f;
                hasDoubleJumped = true;
            }
        }

        Rotate(inputs.LookInput);
        Move(inputs.MoveInput);

        if (inputs.FireInput)
        {
            weaponHandler.FireWeapon();
        }
        else 
        {
            weaponHandler.StopFireWeapon();
        }
        if (inputs.ReloadInput)
        {
            weaponHandler.ReloadWeapon();
        }
        if (inputs.SprintInput)
        {
            StartDash();
        }
        if (inputs.DropInput)
        {
            weaponHandler.DropButton();
        }

        //Dash variable regulation
        if (Dashes < maxDashLimit)
        {
            dashCharge += Time.deltaTime; //This one says if you don't have the max dash charges, build up to a new one
        }
        if (dashCharge >= dashChargeTime)
        { 
            dashCharge = 0;
            Dashes++; //This one says if you have enough charge for a new dash, store the new dash
        }

        if (inputs.InteractInput)
        {
            Interact();
        }

        TakeScreenShot();

        TimerDecrement();
    }

    private void CheckHeadBump()
    {
        if (Physics.Raycast(headPoint.position, headPoint.up, 0.1f))
        {
            //Debug.Log("Did HIT");
            if (verticalForce > 0)
                verticalForce = 0;
        }
    }

    private void TakeScreenShot()
    {
        bool saveAsJPEG = true;
        int imageWidth = 1024;
        if (Input.GetKeyDown(KeyCode.H))
        {
            byte[] bytes = I360Render.Capture(imageWidth, saveAsJPEG);
            if (bytes != null)
            {
                string path = Path.Combine(Application.dataPath, "Scripts/Player/360Captures", "360render" + (saveAsJPEG ? ".jpeg" : ".png"));
                File.WriteAllBytes(path, bytes);
                Debug.Log("360 render saved to " + path);
            }
        }
    }


    //LB: Any and all future timers for the player will be managed in this area
    private void TimerDecrement()
    {

    }
    #region Interactions
    // RL: This code shoots a raycast to allow the player to interact with objects using the IInteractable interace
    private void Interact()
    {
        if (!canInteract) return;

        canInteract = false;

        // Use the player's camera as the ray origin/direction
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found for interaction!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // Raycast to check if we hit something interactable
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
        StartCoroutine(InteractionCooldown());
    }

    private IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }

    //LB: This function will confirm if the interact text should be shown or not
    public void CheckInteract()
    {
        //Debug.Log("PLAYERCONTROLLER - Interactable Check");
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found for interaction!");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // Raycast to check if we hit something interactable
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableMask))
        {
            IInteractable interact = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactionText != null && interact.canInteract)
            {
                //Debug.Log("PLAYERCONTROLLER - Interactable true");
                interactionText.enabled = true;
                interactionText.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log("PLAYERCONTROLLER - CanInteract False - Interactable false");
                interactionText.enabled = false;
                interactionText.gameObject.SetActive(false);
            }
        }
        else
        {
            //Debug.Log("PLAYERCONTROLLER - Interactable false");
            if (interactionText != null)
            {
                interactionText.enabled = false;
                interactionText.gameObject.SetActive(false);
            }
        }
    }

    public void LeftInteract()
    {
        //Debug.Log("PLAYERCONTROLLER - Interactable false");
        if (interactionText != null)
        {
            interactionText.enabled = false;
            interactionText.gameObject.SetActive(false);
        }
    }
    #endregion
    // RL: Code related to achievments and meta-progression
    #region Achievement Code
    private void OnEnable()
    {
        GameEvent.OnAchivementEarned += ApplyAchievementRewards;
    }

    public void ApplyAchievementRewards()
    {
        if (AchievementManager.Instance != null)
        {
            // Check relevant achivements for any bonus the player has acquired

            // Check if player is allowed to double jump
            canDoubleJump = false;
            if (AchievementManager.Instance.CheckAchivementStatus("max_style"))
            {
                canDoubleJump = true;
            }
            // Check for any dash cooldown reduction
            dashCooldownReduction = 0;
            if (AchievementManager.Instance.CheckAchivementStatus("kill_boomba_1"))
            {
                dashCooldownReduction++;
            }
            if (AchievementManager.Instance.CheckAchivementStatus("kill_boomba_2"))
            {
                dashCooldownReduction++;
            }
            // Check for any additional dashes
            bonusDashes = 0;
            if (AchievementManager.Instance.CheckAchivementStatus("kill_boomba_3"))
            {
                bonusDashes++;
            }

            // Update values based on achievments
            dashChargeTime = baseDashChargeTime - (0.25f * dashCooldownReduction);
            maxDashLimit = baseDashCount + bonusDashes;
            // Rebuild Dash UI according to the new current number of dashes
            DashUI[] dashUIs = FindObjectsOfType<DashUI>();
            foreach (DashUI dashUI in dashUIs)
            {
                dashUI.InstantiateDashContainers();
            }
        }
        else
        {
            Debug.LogWarning("PlayerController: Achivement Manager not found. Cannot apply any potential Achievment based abilities.");
        }
    }
    #endregion
}
