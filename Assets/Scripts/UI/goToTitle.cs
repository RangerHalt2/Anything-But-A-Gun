using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;

public class goToTitle : MonoBehaviour 
{

    private LoadingScene loadingScene;


    private void Start()
    {
        loadingScene = GameObject.FindAnyObjectByType<LoadingScene>();
    }

    public void OnButtonClick ()
    {
        GameEvent.RunEnded?.Invoke();
        loadingScene = GameObject.FindAnyObjectByType<LoadingScene>();
        loadingScene.LoadScene("Title Screen");
    }
    
}