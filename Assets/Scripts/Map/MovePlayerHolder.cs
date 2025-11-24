using UnityEngine;

public class MovePlayerHolder : MonoBehaviour
{
    private Transform playerHolder;

    void Start()
    {
        GameObject holder = GameObject.Find("PlayerHolder");
        if (holder != null)
        {
            playerHolder = holder.transform;
            playerHolder.position = transform.position;
        }
    }
}