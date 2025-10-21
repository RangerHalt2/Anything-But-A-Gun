using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;
//Script created By Anthony Mota
public class TitlePage : MonoBehaviour 
{

    public void PlayGame ()
    {
        SceneManager.LoadScene("level1.2");
    }
    public void QuitGame()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
}