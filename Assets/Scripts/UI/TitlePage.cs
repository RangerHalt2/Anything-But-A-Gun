using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;
//Script created By Anthony Mota
public class TitlePage : MonoBehaviour 
{
    [SerializeField] private LoadingScene loader;

    public void PlayGame ()
    {
        Time.timeScale = 0f;
        CleanupDontDestroyOnLoad.DestroyAllDontDestroyOnLoad();
        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if(controller != null)
        {
            Destroy(controller.gameObject);
        }

        GameEvent.RunEnded?.Invoke();

        GameObject canvas = GameObject.Find("Master Canvas");
        

        GameEvent.RunStarted?.Invoke();

        loader.LoadScene("Level Gen 5");
        Destroy(canvas);
    }
    public void QuitGame()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}