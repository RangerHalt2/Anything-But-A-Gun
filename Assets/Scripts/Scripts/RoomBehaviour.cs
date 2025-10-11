using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls;
    public GameObject[] doors;
    public bool[] testStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateRoom(testStatus);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }

    }

}
