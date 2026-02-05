// Created By: Anthony Mota
using UnityEngine;
using System.Collections;

public class FloatAndRotate : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.25f;
    public float frequency = 1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    public RotationAxis rotationAxis = RotationAxis.Y;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;

    private WeaponCollectScript weaponCollectScript;
    private Coroutine floatRoutine;

    public enum RotationAxis { X, Y, Z }

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;

        weaponCollectScript = GetComponentInParent<WeaponCollectScript>();
        StartFloatAndRotate();
    }

    public void StartFloatAndRotate()
    {
        floatRoutine = StartCoroutine(FloatAndRotateRoutine());
    }

    IEnumerator FloatAndRotateRoutine()
    {
        while (true)
        {
            // If the object has a weapon collect script
            if(weaponCollectScript != null)
            {
                // Wait until weapon is not collected
                while (!weaponCollectScript.collected)
                {
                    // Floating motion
                    float newY = startLocalPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
                    transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

                    // Rotation motion
                    Vector3 axis = rotationAxis switch
                    {
                        RotationAxis.X => Vector3.right,
                        RotationAxis.Y => Vector3.up,
                        RotationAxis.Z => Vector3.forward,
                        _ => Vector3.up
                    };

                    transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.Self);

                    yield return null;
                }
            }
            // If there is no weapon collect script
            else
            {
                // Spin eternally
                while(true)
                {
                    // Floating motion
                    float newY = startLocalPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
                    transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

                    // Rotation motion
                    Vector3 axis = rotationAxis switch
                    {
                        RotationAxis.X => Vector3.right,
                        RotationAxis.Y => Vector3.up,
                        RotationAxis.Z => Vector3.forward,
                        _ => Vector3.up
                    };

                    transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.Self);

                    yield return null;
                }
            }
            
            // When collected, reset position & rotation
            transform.localPosition = startLocalPos;
            transform.localRotation = startLocalRot;

            // Wait until weapon is no longer collected before resuming
            yield return new WaitUntil(() => weaponCollectScript != null && !weaponCollectScript.collected);
        }
    }

    void OnDisable()
    {
        if (floatRoutine != null)
            StopCoroutine(floatRoutine);
    }
}
