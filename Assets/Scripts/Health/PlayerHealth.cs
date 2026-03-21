using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private TMP_Text healthText;

    [Header("UI References")]
    [SerializeField] private Image healthBar;         // Filled Image
    [SerializeField] private GameObject gameOverScreen;

    [Header("Color Gradient")]
    [SerializeField] private Gradient healthGradient;

    public EndingGrade end; // added by Aaron

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
            HandleGameOver();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

  private void UpdateHealthUI()
{
    if (healthBar != null)
    {
        float healthPercent = (float)currentHealth / maxHealth;
        healthBar.fillAmount = healthPercent;
        healthBar.color = healthGradient.Evaluate(healthPercent);
    }

    if (healthText != null)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}

    private void HandleGameOver() // I thought this was the part of the code that handled the game over but i was incorrect - Aaron
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("HELLOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
            end.EndingGradeCoroutine(); // Added by Aaron
            Debug.Log("ENDING COROUTINE ATTEMPT");
    }

    public int GetHealth() => currentHealth;
}
