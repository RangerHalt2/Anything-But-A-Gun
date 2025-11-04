using UnityEngine;

public class ObjectGravity : MonoBehaviour
{
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float groundDistance = 0.05f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundPoint;
    
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void DoGravity()
    {

    }

    private bool IsGrounded()
    {
        bool ret = false;

        if(Physics.Raycast(groundPoint.position, Vector3.down, groundDistance, whatIsGround))
        {
            ret = true;
        }

        return ret;
    }

}
