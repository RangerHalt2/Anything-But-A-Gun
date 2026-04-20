using UnityEngine;
using System.Collections;

public class CutoutFall : MonoBehaviour
{

    public float fallenAngle = 90f;
    public float fallSpeed = 2f;
    public bool hasFallen = false;

    public GameObject fallSensor;

    private Quaternion _uprightRotation;
    private Quaternion _fallenRotation;
    private Coroutine _fallingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _uprightRotation = transform.rotation;
        _fallenRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, fallenAngle));
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasFallen)
        {
            if (_fallingCoroutine != null) StopCoroutine(_fallingCoroutine);
            _fallingCoroutine = StartCoroutine(CutoutFalling());
        }
    }

    private IEnumerator CutoutFalling()
    {
        hasFallen = true;
        
        Quaternion targetRotation = _fallenRotation;

        float degreesPerSecond = fallSpeed * 50f; 

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                degreesPerSecond * Time.deltaTime
            );
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
