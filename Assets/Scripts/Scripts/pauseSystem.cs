using UnityEngine;

public class pauseSystem : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu = null;
    bool isPaused;

   
  
    public void PauseGame()
    {
        // Pauses the game by setting the time scale to 0
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }
    public void UnpauseGame()
    {
        
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }
}
