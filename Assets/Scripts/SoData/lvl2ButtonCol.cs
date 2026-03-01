using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class lvl2ButtonCol : MonoBehaviour
{
   
    [SerializeField]
    public unlockButton2 button2;
    public GameObject lvl2Door;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button2.Value = false;
            Debug.Log(+1);
           
            lvl2Door.SetActive(false);
        }
    }
}
