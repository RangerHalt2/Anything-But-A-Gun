using UnityEngine;

public class LevelStarted : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        PlayerController playerController = GameObject.FindAnyObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.isSpawned = true;
        }

    }
}
