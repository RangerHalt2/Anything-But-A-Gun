using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;

public class goToTitle : MonoBehaviour 
{

    public void OnButtonClick ()
    {
        SceneManager.LoadScene("Title Screen");
    }
    
}