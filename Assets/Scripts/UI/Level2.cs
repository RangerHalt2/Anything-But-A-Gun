using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement; 
using TMPro;
//Script created By Anthony Mota
public class Level2 : MonoBehaviour 
{

    public void PlayGame ()
    {
        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if(controller != null)
        {
            Destroy(controller.gameObject);
        }

        SceneManager.LoadScene("Level Gen 3");
    }
   
}