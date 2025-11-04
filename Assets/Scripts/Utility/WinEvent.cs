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

    public bool hasWon = false;

    public void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("Win Event: UI Manager could not be found in current scene!");
        }

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
        // If it is the final level
        if (isFinalLevel)
        {
            // If the UI Manager exists
            if (uiManager != null)
            {
                uiManager.TogglePause();
                // Pull up the winPage
                uiManager.GoToPage(winPageIndex);
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
                other.transform.position = nextScenePos;

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
