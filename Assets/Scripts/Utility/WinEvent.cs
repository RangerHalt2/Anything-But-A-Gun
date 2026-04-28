using Unity.VisualScripting;
using UnityEngine;

public class WinEvent : MonoBehaviour
{

    [Tooltip("The Determines what the win trigger will do. If true, it will pull up the win screen. If false, it will move the player to scene containing the next level.")]
    [SerializeField] private bool isFinalLevel;
    // Reference to the UI Manager
    private UIManager uiManager;
    [Tooltip("The index of the win page in the UI Manager")]
    [SerializeField] private int winPageIndex;
    // Reference to the scene controller
    private SceneController sceneController;
    [Tooltip("The name of the scene containing the next level.")]
    [SerializeField] private string nextSceneName;
    [Tooltip("The position of the player in the next level.")]
    [SerializeField] private Vector3 nextScenePos;
    public WaveManager doorOpen;
    public gammaSlider volume;

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] public GameObject inGameCanvas;

    public EndingGrade end; // added by Aaron
    private bool endScorePlaying; // added by Aaron

    [SerializeField]
    public unlockButton2 button2;

    public bool hasWon = false;

    public void Start()
    {
        uiManager = GameObject.FindAnyObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("Win Event: UI Manager could not be found in current scene!");
        }

        if (GameObject.FindAnyObjectByType<StyleGaugeController>() != null)
            inGameCanvas = GameObject.FindAnyObjectByType<StyleGaugeController>().gameObject;

        if (GameObject.FindAnyObjectByType<EndingGrade>() != null)
            gameOverCanvas = GameObject.FindAnyObjectByType<EndingGrade>().gameObject;

        sceneController = FindFirstObjectByType<SceneController>();
        if (sceneController == null)
        {
            Debug.LogWarning("Win Event: Scene Controller could not be found in current scene!");
        }
        // Ensure hasWon is false at the start of any given scene
        hasWon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ensure that the game object entering the trigger is the player
        if (other.gameObject.CompareTag("Player"))
         {
            if (button2 != null)
                button2.Value = 1;
            // If it is the final level
            if (isFinalLevel)
            {
                // If the UI Manager exists
                if (uiManager != null)
                {
                    //uiManager.TogglePause();
                    // Pull up the winPage
                    //uiManager.GoToPage(winPageIndex); //LB: Deprecated, using the ending score now.

                    if (GameObject.FindAnyObjectByType<StyleGaugeController>(FindObjectsInactive.Include) != null)
                        inGameCanvas = GameObject.FindAnyObjectByType<StyleGaugeController>(FindObjectsInactive.Include).gameObject;

                    if (GameObject.FindAnyObjectByType<EndingGrade>(FindObjectsInactive.Include) != null)
                        gameOverCanvas = GameObject.FindAnyObjectByType<EndingGrade>(FindObjectsInactive.Include).gameObject;
                    else Debug.Log("WIN EVENT - The EndingGrade was null");

                    gameOverCanvas.SetActive(true);
                    inGameCanvas.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;

                    PlayerController player = GameObject.FindAnyObjectByType<PlayerController>();
                    if (player != null) player.hasWon = true;

                    end = GameObject.FindAnyObjectByType<EndingGrade>();
                    if (end != null && endScorePlaying != true) // added endScorePlaying variable to make sure the ending stuff only runs once
                    {
                        endScorePlaying = true;
                        StartCoroutine(end.EndingGradeCoroutine());
                        Debug.Log("Win Event - END ISNOT NULL AND COROUTINE IS STARTING");
                    }
                    else
                        Debug.Log("Win Event - END IS NULL");

                    // Set timeScale to 0 (to stop enemies form killing the player)
                    Time.timeScale = 0f;
                    // Unlock Cursor
                    Cursor.lockState = CursorLockMode.None;

                    // Set hasWon to true. This is referenced by other scripts and I didn't want to risk messing up other code by removing it entirely - Ryan
                    //hasWon = true;
                }
                else
                {
                    Debug.LogWarning("Win Event: UI Manager could not be found in current scene! Cannot open winPage!");
                }
            }
            // If it is not the final level
            else
            {
                if (sceneController != null)
                {
                    // Move player to the specified position in the next scene
                    GameObject player = other.transform.parent != null ? other.transform.parent.gameObject : other.gameObject;
                    PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
                    controller.isSpawned = false;
                    player.transform.position = nextScenePos;
                    Debug.Log(other.transform.position);
                    volume = null;

                    if(nextSceneName == "Level Gen 3")
                    {
                        GameEvent.OnLevelCompleted?.Invoke("office");
                    }
                    else if (nextSceneName == "XL_Staircase")
                    {
                        GameEvent.OnLevelCompleted?.Invoke("lab");
                    }
                    // Go to the next scene
                    sceneController.GoToScene(nextSceneName);
                }
                else
                {
                    Debug.LogWarning("Win Event: Scene Controller could not be found in current scene! Cannot load next scene!");
                }
            }
        }
    }
}
