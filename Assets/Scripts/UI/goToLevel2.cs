using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;

public class goToLevel2 : MonoBehaviour 
{

    public void OnButtonClick ()
    {
        SceneManager.LoadScene("LEVEL 2");
    }
    
}