using UnityEngine;
using System.Collections;

public class ObjectHover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float heightOffset = 1.0f; 
    [SerializeField] float moveDuration = 1.0f; 
    [SerializeField] float pauseDuration = 2.0f;
    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + new Vector3(0, heightOffset, 0);
        StartCoroutine(HoverRoutine());
    }

    IEnumerator HoverRoutine()
    {
        while (true)
        {
            // Move Up
            yield return StartCoroutine(MoveObject(startPosition, endPosition));
            // Pause at top
            yield return new WaitForSeconds(pauseDuration);
            // Move Down
            yield return StartCoroutine(MoveObject(endPosition, startPosition));
            // Pause at bottom
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    IEnumerator MoveObject(Vector3 start, Vector3 end)
    {
        float timeElapsed = 0;
        while (timeElapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(start, end, timeElapsed / moveDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end; 
    }
}