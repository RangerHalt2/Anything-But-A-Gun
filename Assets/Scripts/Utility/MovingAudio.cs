using UnityEngine;

public class MovingAudio : MonoBehaviour
{
    public Transform targetToFollow;

    private void Update()
    {
        if(targetToFollow != null)
        {
            transform.position = targetToFollow.position;
        }
    }
}
