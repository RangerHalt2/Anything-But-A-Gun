using UnityEngine;

public class activeButtonLvl2 : MonoBehaviour
{
    [SerializeField]
    public unlockButton2 button2;
    public GameObject lvl2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()

    {
        

        if (button2.Value >=1)

        {
            lvl2.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
