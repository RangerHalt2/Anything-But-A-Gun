using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;

public class goToTitle : MonoBehaviour 
{

    public void OnButtonClick ()
    {
        GameEvent.RunEnded?.Invoke();

        SceneManager.LoadScene("Title Screen");
    }
    
}