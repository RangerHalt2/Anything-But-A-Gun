using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;
//Script created By Anthony Mota
public class TitlePage : MonoBehaviour 
{

    [SerializeField] private LoadingScene loader;

    private void Update()
    {
        if(loader == null)
        {
            loader = GameObject.FindAnyObjectByType<LoadingScene>();
        }
    }

    public void PlayGame ()
    {
        GameEvent.RunEnded?.Invoke();

        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if(controller != null)
        {
            Destroy(controller.gameObject);
        }

        GameObject canvas = GameObject.Find("Master Canvas");

        loader.LoadScene("Level Gen 5");
        CleanupDontDestroyOnLoad.DestroyAllDontDestroyOnLoad(transform.root.gameObject);

        GameEvent.RunStarted?.Invoke();
    }
    public void QuitGame()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}