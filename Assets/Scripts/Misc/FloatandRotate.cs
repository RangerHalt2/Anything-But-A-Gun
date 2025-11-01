// Created By: Anthony Mota
using UnityEngine;

public class FloatAndRotate : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.25f;
    public float frequency = 1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
