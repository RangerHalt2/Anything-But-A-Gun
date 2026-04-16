using UnityEngine;

public class ShakeTest : MonoBehaviour
{

    private CameraShake cameraShake;

    private void Start()
    {
        cameraShake = GameObject.FindAnyObjectByType<CameraShake>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("SHAKE TEST - attempting to shake the camera");
            cameraShake.Shake(new CameraShake.ShakeConfig
            {
                duration = 0.4f,
                impactX = 3f,
                impactY = 2f,
                impactZ = 5f,
                frequencyZ = 8f   // Hz — higher = faster back-and-forth
            });
        }
    }

}
