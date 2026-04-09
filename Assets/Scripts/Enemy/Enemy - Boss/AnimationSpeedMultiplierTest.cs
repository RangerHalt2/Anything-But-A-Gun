using UnityEngine;

public class AnimationSpeedMultiplierTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on this GameObject!");
            return;
        }

        animator.SetFloat("SpeedMultiplier", 3f);
    }
}