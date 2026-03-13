using System.Collections;
using UnityEngine;

public class RotateAndReturn : MonoBehaviour
{
    // The angle to rotate on the X axis
    public float targetAngleX = 90f;
    // The speed of rotation
    public float rotationSpeed = 3f;

    private Quaternion originalRotation;
    private bool isRotating = false;

    void Start()
    {
        // Store the original rotation when the object starts
        originalRotation = transform.rotation;
        StartCoroutine(RotateObject());

    }

    void Update()
    {
      
    }

    IEnumerator RotateObject()
    {
        isRotating = true;

        Quaternion targetRotation = originalRotation * Quaternion.Euler(targetAngleX, 0f, 0f); 
        yield return StartCoroutine(SmoothRotateTo(targetRotation));

        yield return StartCoroutine(SmoothRotateTo(originalRotation));

        isRotating = false;
    }

    IEnumerator SmoothRotateTo(Quaternion endRotation)
    {
        float t = 0f;
        Quaternion startRotation = transform.rotation;

        while (t < 1f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            t += Time.deltaTime * rotationSpeed;
            yield return null; // Wait for the next frame
        }

        transform.rotation = endRotation;
    }
}