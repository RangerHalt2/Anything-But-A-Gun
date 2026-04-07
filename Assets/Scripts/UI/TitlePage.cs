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
        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if(controller != null)
        {
            Destroy(controller.gameObject);
        }

        GameEvent.RunEnded?.Invoke();

        GameObject canvas = GameObject.Find("Master Canvas");
       
        GameEvent.RunStarted?.Invoke();

        loader.LoadScene("Level Gen 5");
        CleanupDontDestroyOnLoad.DestroyAllDontDestroyOnLoad(transform.root.gameObject);
    }
    public void QuitGame()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}