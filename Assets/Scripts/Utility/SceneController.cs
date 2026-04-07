using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //Dynamic Scene Naming capabilities
    [SerializeField] private string MainMenu = "";
    [SerializeField] private LoadingScene loader;

    void Start()
    {
        if (loader == null)
            loader = GameObject.FindAnyObjectByType<LoadingScene>();
    }

    // Closes out of the application
    public void Quit()
    {
        Application.Quit();
    }

    public void GoToScene(String sceneName)
    {
        if(loader.LoadingScreen != null)
            loader.LoadingScreen.SetActive(true);
        loader.LoadScene(sceneName);
        return;
    }

    public void GoToMainMenu()
    {
        loader.LoadScene("TitleScreen");
        SceneManager.LoadScene(MainMenu);
    }

    //Hard Coded Scene Management for testing scene
    public void GoToCombatTestScene()
    {
        SceneManager.LoadScene("CombatTestScene");
    }

}
