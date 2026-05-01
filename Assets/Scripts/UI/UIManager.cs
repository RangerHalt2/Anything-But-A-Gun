// Created By: Ryan Lupoli
// Manages the UI during gameplay and allows for easy navigation between pages
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem; 

public class UIManager : MonoBehaviour
{
    #region Variables
    public static UIManager instance;
    [Header("Page Management")]
    [Tooltip("The Pages (or Panels) managed by the UI Manager.")]
    public List<UIPage> pages;
    [Tooltip("The index of the currently active page in the UI.")]
    public int currentPage = 0;
    [Tooltip("The index of the page the UI should start on when the UI Manager starts up.")]
    public int defaultPage = 0;
    private int previousPage = 0;

    [Header("Pause Settings")]
    [Tooltip("Reference to InputActions Asset.")]
    [SerializeField] private InputActionAsset UIControls;
    private InputManager inputManager;
    // Reference to pause action from the InputActionsAsset
    private InputAction pauseAction;
    [Tooltip("The index of the pause page in the pages list.")]
    public int pausePageIndex = 1;
    [Tooltip("Determines whether or not the player is allowed to pause. Set to true to enable pausing.")]
    public bool allowPause = true;
    // Whether or not the game is currently paused
    public bool isPaused = false;
    public bool IsPaused => isPaused;
    public bool IsTitleScreen = false;

    [SerializeField] private GameObject defaultSelectedPauseObject;
    [SerializeField] private GameObject[] pauseTabButtons;

    private Stack<int> pageHistory = new Stack<int>();

    [SerializeField] private Canvas quitConfirmCanvas;
    [SerializeField] private GameObject confirmButton;

    #endregion

    void Awake()
    {
        Time.timeScale = 1f;
        // Ensure there is only one instance of the UI Manager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Destroy(this);
        }

        // Get the pause action from the UI Action Map in InputActions
        var uiMap = UIControls.FindActionMap("UI", true);
        pauseAction = uiMap.FindAction("Pause", true);

        pauseAction.performed += OnPausePerformed;
        UIControls.Enable();

        //EW: Added to fix the kill screen. See Die() in Health for more information
        allowPause = true;
    }


    private void OnEnable()
    {
        Debug.Log("UI MANAGER - OnEnable fired, subscribing to pause action");
        pauseAction.performed += OnPausePerformed;
        UIControls.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        UIControls.Disable();
    }

    private void OnDestroy()
    {
        Debug.Log("MASTER CANVAS - Destroyed! " + System.Environment.StackTrace);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitilizeFirstPage();
        inputManager = GameObject.FindAnyObjectByType<InputManager>();
        EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
    }

    // Sets up the first page. Ensures that only the default page is enabled on startup
    private void InitilizeFirstPage()
    {
        GoToPage(defaultPage);
        currentPage = pausePageIndex;
    }


    private void Update()
    {
        //This handles the return button on the controller:
        if (inputManager != null && inputManager.ControllerBack && isPaused)
        {
            Debug.Log("UI MANAGER - Controller Back ran");
            inputManager.ControllerBack = false;
            if(pageHistory.Count <= 0) // Base case for empty stack
            {
                TogglePause();
                return;
            }
            int poppedPage = pageHistory.Pop();
            if(poppedPage == 1 || currentPage == 1)
            {
                TogglePause();
                return;
            }
            Debug.Log("UI MANAGER - page history: after pop ");
            PrintStack();
            GoToPage(poppedPage);
            Debug.Log("UI MANAGER - page history: before second pop ");
            PrintStack();
            pageHistory.Pop();
            Debug.Log("UI MANAGER - page history: after second pop ");
            PrintStack();
            if (currentPage == 7)
                EventSystem.current.SetSelectedGameObject(pauseTabButtons[0]);
            else if (currentPage == 8)
                EventSystem.current.SetSelectedGameObject(pauseTabButtons[1]);
            else if (currentPage == 9)
                EventSystem.current.SetSelectedGameObject(pauseTabButtons[2]);
        }

        if (IsTitleScreen)
        {
            if(defaultSelectedPauseObject != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
        }
    }

    // Toggles whether or not the game is currently paused
    public void TogglePause()
    {
        Debug.Log("UI MANAGER - Toggling Pause: " + !isPaused);
        pageHistory.Clear();
        // If pausing is allowed
        if (allowPause && !IsTitleScreen)
        {
            // If the game is currently paused, un-pause it
            if (isPaused)
            {
                // If the current page is the pause menu, un-pause the game
                if (currentPage == pausePageIndex)
                {
                    // Go to the default UI Page
                    GoToPage(defaultPage);
                    currentPage = defaultPage;
                    // Set time scale to 1 (normal speed)
                    Time.timeScale = 1f;
                    // Lock the cursor
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    // Update pause boolean
                    isPaused = false;

                    Health playerHealth = GameObject.FindAnyObjectByType<PlayerController>().GetComponent<Health>();
                    playerHealth.updateDisplay();
                    return;
                }
                // If the current page is not the pause menu, then try to go to the previous page
                else
                {
                    // If current Page is 7, 8 , or 9
                    if(currentPage == 7 || currentPage == 8 || currentPage == 9)
                    {
                        // Go to pause page
                        GoToPage(pausePageIndex);
                        EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
                    }
                    // If any other page, go to the previous page
                    else
                    {
                        GoToPage(previousPage);
                        return;
                    }
                }
            }
            // If the game is not currently paused, pause it
            else
            {
                // Go to pause UI Page
                EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
                previousPage = currentPage;
                GoToPage(pausePageIndex);
                currentPage = pausePageIndex;
                // Set time scale to 0 (frozen)
                Time.timeScale = 0f;
                // Lock the cursor
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // Update pause boolean
                isPaused = true;
                return;
            }
        }
        else
        {
            if (IsTitleScreen && allowPause)
            {
                ShowQuitConfirm(); 
                return;
            }
        }
    }

    // Go to a page by the page's index
    public void GoToPage(int pageIndex)
    {
        // If the page index is within the bounds of pages, and a page has been assigned at that index
        if (pageIndex < pages.Count && pages[pageIndex] != null)
        {
            // Disable all pages
            SetActiveAllPages(false);
            // Activate the specified page
            previousPage = currentPage;
            pages[pageIndex].gameObject.SetActive(true);
            currentPage = pageIndex;
            pageHistory.Push(previousPage);
            Debug.Log("UI MANAGER - page history & added: ");
            PrintStack();
        }
    }

    //For debugging purposes this prints the Stack History
    private void PrintStack()
    {
        string history = "";
        foreach(int i in pageHistory)
        {
            history += i.ToString()+"\n";
        }
        Debug.Log("UI MANAGER - History:\n" + history);
    }

    // Turns all pages on or off according to the activated parameter
    public void SetActiveAllPages(bool activated)
    {
        // If pages has at least one page assinged
        if (pages != null)
        {
            // For every UIPage in the list
            foreach (UIPage page in pages)
            {
                if (page != null)
                {
                    // Activate or deactivate the page according to the state of activated
                    page.gameObject.SetActive(activated);
                }
            }
        }
    }

    private void ShowQuitConfirm()
    {
        if(quitConfirmCanvas != null)
        {
            Debug.Log("UI MANAGER - Showing the Quit Confirm Now!");
            Cursor.lockState = CursorLockMode.None;
            quitConfirmCanvas.gameObject.SetActive(true);
            StartCoroutine(SetSelectedNextFrame(confirmButton));
        }
    }

    private IEnumerator SetSelectedNextFrame(GameObject target)
    {
        yield return null; // Wait one frame
        EventSystem.current.SetSelectedGameObject(target);
    }

    public void RejectQuit()
    {
        if (quitConfirmCanvas != null)
        {
            quitConfirmCanvas.gameObject.SetActive(false);
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePause();
        }
    }
}
