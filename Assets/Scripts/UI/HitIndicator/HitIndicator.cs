using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    [SerializeField] private float radius;

    private Transform enemyLocation;
    private Transform playerLocation;

    private Vector3 lastKnownPosition;

    [SerializeField] private RectTransform arrow;


    public void Initialize(Transform enemySource)
    {
        enemyLocation = enemySource;
        playerLocation = GameObject.FindAnyObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        if (enemyLocation != null || lastKnownPosition != null)
        {
            CheckRotation();
        }
    }

    // Checks and updates the rotation based on the transform, this should be called in the Update Function
    // Converts 3D coordinates to 2D visual representation. Forward is up, Backwards is down. Left and Right consistent.
    private void CheckRotation()
    {
        
        Vector3 localDir = enemyLocation != null ? playerLocation.InverseTransformDirection(enemyLocation.position - playerLocation.position) : playerLocation.InverseTransformDirection(lastKnownPosition - playerLocation.position);
        Vector2 hitDir2D = new Vector2(localDir.x, localDir.z).normalized;

        arrow.anchoredPosition = hitDir2D * radius;

        float angle = Mathf.Atan2(hitDir2D.x, hitDir2D.y) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
        if (enemyLocation != null)
        {
            lastKnownPosition = enemyLocation.position;
        }
    }

}
