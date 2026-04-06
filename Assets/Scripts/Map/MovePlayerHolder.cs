using System.Collections;
using UnityEngine;

public class MovePlayerHolder : MonoBehaviour
{
    private Transform playerHolder;

    void Start()
    {
        Debug.LogWarning("Code Started; Attempting to Find Player Holder");
        PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
        if (controller != null)
        {   
            Debug.LogWarning("Player Holder FOUND");
            StartCoroutine(LateStart(controller));
        }
        else
        {
            Debug.LogWarning("Player Holder NOT FOUND");
        }
    }

    private IEnumerator LateStart(PlayerController controller)
    {
        yield return new WaitForSeconds(0.2f);

        playerHolder = controller.transform;

        CharacterController cc = controller.GetComponent<CharacterController>();
        if (cc == null)
        {
            Debug.LogWarning("The character controller could not be found");
            yield return null;
        }
        cc.enabled = false;
        playerHolder.position = transform.position;
        cc.enabled = true;
        controller.isSpawned = true;

        if(GameObject.FindAnyObjectByType<LoadingIndicator>() != null)
        {
            GameObject loadingScreen = GameObject.FindAnyObjectByType<LoadingIndicator>().gameObject;
            loadingScreen.SetActive(false);
        }

    }
}