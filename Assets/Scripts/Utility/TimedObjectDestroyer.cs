// Created By: Ryan Lupoli
// Automatically destroys a game object after a set delay
using UnityEngine;
using UnityEngine.UI;

public class TimedObjectDestroyer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("The amount of time (in seconds) before the object this script is attatched to is destroyed.")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float thresholdToFade = 2f;
    [SerializeField] private bool doesFade = false;

    private Renderer r_renderer;
    private CanvasGroup canvasGroup;
    private Image images;

    // The amount of time which has passed since this script was initialized.
    private float elapsedTime = 0;
    private float progress;

    private void Start()
    {
        r_renderer ??= GetComponentInChildren<Renderer>();
        r_renderer ??= GetComponentInParent<Renderer>();

        canvasGroup ??= GetComponent<CanvasGroup>();
        canvasGroup ??= GetComponentInChildren<CanvasGroup>();

        images ??= GetComponentInChildren<Image>();
        images ??= GetComponentInParent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // Track time which has passed
        elapsedTime += Time.deltaTime;
        progress = (elapsedTime / lifeTime);

        if (doesFade && elapsedTime >= thresholdToFade)
        {
            if (r_renderer != null) Fade3DObjects();
            if (canvasGroup != null) FadeCanvasGroup();
            if (images != null) FadeUIImages();
        }

        // Once the lifeTime of the object has passed...
        if (elapsedTime >= lifeTime)
        {
            // Destroy the game object
            Debug.Log("Timed Object Destroyer: Destroyed " + gameObject.name + ".");
            Destroy(gameObject);
        }
    }

    private void Fade3DObjects()
    {
        Color color = r_renderer.material.color;
        color.a = Mathf.Lerp(1, 0, Mathf.Clamp01(progress));
        r_renderer.material.color = color;
    }

    private void FadeUIImages()
    {
        Color color = images.color;
        color.a = Mathf.Lerp(1, 0, Mathf.Clamp01(progress));
        images.color = color;
    }

    private void FadeCanvasGroup()
    {
        canvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01(progress));
    }
}
