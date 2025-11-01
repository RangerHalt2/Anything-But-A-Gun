using UnityEngine;
using System.Collections;

public class WeaponTransformManager : MonoBehaviour
{
    [Header("Model Reference")]
    [Tooltip("Refrence to the transform of the Weapon's model.")]
    [SerializeField] private Transform modelTransform;

    [Header("Equipped Local Transform")]
    [Tooltip("The local position of the weapon when equipped.")]
    [SerializeField] private Vector3 equippedLocalPosition;
    [Tooltip("The local rotation of the weapon when equipped.")]
    [SerializeField] private Vector3 equippedLocalRotation;

    [Header("Grounded Local Transform")]
    [Tooltip("The local position of the weapon when on the ground.")]
    [SerializeField] private Vector3 groundLocalPosition;
    [Tooltip("The local rotation of the weapon when on the ground.")]
    [SerializeField] private Vector3 groundLocalRotation;

    [Header("Transition Settings")]
    [Tooltip("Determines if the weapon should have a smooth transition to the new transform. Set to true for smooth transition.")]
    public bool smoothTransition = true;
    [Tooltip("The speed at which the weapon changes its transforms")]
    public float transitionSpeed = 10f;

    private Coroutine moveRoutine;

    private void OnDisable()
    {
        // When disabled, stop the move routine.
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
    }

    // Sets the weapon to its equipped transform
    public void SetEquipped()
    {
        // Stop the move Routine if its active
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        // If smooth transition is enabled...
        if (smoothTransition)
        {
            // Start coroutine to move weapon
            moveRoutine = StartCoroutine(MoveToLocal(modelTransform, equippedLocalPosition, Quaternion.Euler(equippedLocalRotation)));
        }
        // If its not...
        else
        {
            // Snap weaponmodel to desired transforms
            SnapToLocal(modelTransform, equippedLocalPosition, Quaternion.Euler(equippedLocalRotation));
        }
    }

    // Sets the weapon to its grounded transform
    public void SetGrounded()
    {
        // Stop the move Routine if its active
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        // If smooth transition is enabled...
        if (smoothTransition)
        {
            // Start coroutine to move weapon
            moveRoutine = StartCoroutine(MoveToLocal(modelTransform, groundLocalPosition, Quaternion.Euler(groundLocalRotation)));
        }
        // If its not...
        else
        {
            // Snap weaponmodel to desired transforms
            SnapToLocal(modelTransform, groundLocalPosition, Quaternion.Euler(groundLocalRotation));
        }
    }

    // Snaps a model to specified Local Transforms
    private void SnapToLocal(Transform target, Vector3 localPos, Quaternion localRot)
    {
        // If target is null, do nothing
        if (target == null) return;
        // Change localPosition to the provided localPos
        target.localPosition = localPos;
        // Change localRotation to the provided localRot
        target.localRotation = localRot;
    }
    
    // Gradually moves a model to the desired Local Transforms
    private IEnumerator MoveToLocal(Transform target, Vector3 targetPos, Quaternion targetRot)
    {
        // If target is null, do nothing
        if (target == null) yield break;

        // While the weapon is approaching the desired transform
        while (Vector3.Distance(target.localPosition, targetPos) > 0.001f ||
               Quaternion.Angle(target.localRotation, targetRot) > 0.1f)
        {
            // Gradually move the position and transform twoards their target values
            target.localPosition = Vector3.Lerp(target.localPosition, targetPos, Time.deltaTime * transitionSpeed);
            target.localRotation = Quaternion.Slerp(target.localRotation, targetRot, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        // Once the mocdel is near its target values, snap it to the desired transform
        SnapToLocal(target, targetPos, targetRot);
    }
}
