// Created By: Ryan Lupoli
// This is a script meant to track a game object's health
using System;
using System.IO;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Variables
    [Header("Team Settings")]
    [Tooltip("Determines the \"Team\" the object is on. Objects on the same team cannot damage each other.")]
    public int teamID;

    [Header("Health Settings")]
    [Tooltip("The maximum amount of health an object can. Value must be greater than 0.")]
    [SerializeField] public float maxHealth = 1f;
    [Tooltip("The amount of health the object currently has. If the current health is 0, the object is dead.")]
    [SerializeField] public float currentHealth = 1f;
    private float initialMaxHealth;

    //EW: For nonlethal damage
    [Tooltip("Whether or not the enemy died from nonlethal damage. Public so other methods can call it.")]
    public bool nonlethalDefeat = false;

    [Header("Display Settings")]
    [Tooltip("Determines if the Health Bar is active before taking damage")]
    [SerializeField] private bool healthBarActiveOnStartup;
    // Tracks if the healthbar is currently active
    private bool healthBarActive;
    [Tooltip("Reference to healthbar prefab. Optional.")]
    [SerializeField] public HealthBar healthBar;
    [Tooltip("Reference to TMPro Object used to track current AP. Optional.")]
    [SerializeField] private TextMeshProUGUI healthDisplayText;

    [Header("Effect Settings")]
    [Tooltip("Reference to prefab for an effect which triggers when the object recieves damage. Optional.")]
    public GameObject hitEffect;
    private float tickTimer;
    private float tick = 0.1f; //Amount of time between VFX instantiations as a stall.
    [Tooltip("Reference to prefab for an effect which triggers when the object is destroyed. Optional.")]
    public GameObject deathEffect;

    [Tooltip("Toggle this if this is on the player")]
    [SerializeField] private bool isPlayer;
    [Tooltip("How much EXP is given to the player on this NPC's death? Doesn't matter if it's the player")]
    [SerializeField] private float EXPDrop;
    private Player_Level playerLevel;

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] public GameObject inGameCanvas;

    public bool isDead;

    [SerializeField] private GameObject damageNoise;
    private float damageTimer = 0f;
    private float damageCooldown = 0.5f;

    [SerializeField] private StyleGaugeController style;

    [HideInInspector] public bool ShiningArmor = false;

    public event Action<DamageInfo> PlayerTookDamage;

    [SerializeField] private ParticleSystem bloodVFX;
    [SerializeField] private int minParticle;
    [SerializeField] private int maxParticle;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    private P_FamilyPhoto familyPhotoPromotion;

    public bool infiniteHealthOn = false;

    #endregion

    public struct DamageInfo
    {
        public Transform source;
        public float damage;
    }

    public EndingGrade end; // added by Aaron
    private bool endScorePlaying; // added by Aaron

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        style = (StyleGaugeController)FindFirstObjectByType(typeof(StyleGaugeController));
        playerLevel = GameObject.FindAnyObjectByType<Player_Level>();
        isDead = false;
        endScorePlaying = false; // added by Aaron
        initialMaxHealth = maxHealth;
        // Automatically kill object if it has 0 or less health

        if (isPlayer)
        {
            if (GameObject.FindAnyObjectByType<HealthBarIndicator>() != null)
            {
                healthBar = GameObject.FindAnyObjectByType<HealthBarIndicator>().GetComponent<HealthBar>();
                Debug.Log("HEALTH - health bar is: " + healthBar);
            }
            else Debug.Log("HEALTH - Could not automatically assign the HealthBar");

            if (GameObject.FindAnyObjectByType<EndingGrade>() != null)
                gameOverCanvas = GameObject.FindAnyObjectByType<EndingGrade>().gameObject;

            if(GameObject.FindAnyObjectByType<StyleGaugeController>() != null)
                inGameCanvas = GameObject.FindAnyObjectByType<StyleGaugeController>().gameObject;
        }

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + "'s Initial Health was equal to or less than 0. They have been automatically destroyed.");
            Die();
        }

        // If there is a healthbar assigned, set the max health
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        updateDisplay();

        // If the health bar is not meant to be active immediatly, deactivate it
        if (healthBar != null && !healthBarActiveOnStartup)
        {
            healthBar.Deactivate();
        }

        // If object is the player
        if (isPlayer)
        {
            ApplyAchievementHealthIncreases();
        }
    }

    // Update is called once per frame
    void Update()
    {
        damageTimer -= Time.deltaTime;
        if (tickTimer > 0) {tickTimer -= Time.deltaTime; }
    }

    // Applies a certain amount of damage to an object
    public void TakeDamage(float damageAmount, Transform sourcePosition)
    {
        if (infiniteHealthOn) return;
        if (ShiningArmor)
        {
            Debug.Log("HEALTH - SHINING ARMOR - damage intercepted");
            ShiningArmor = false;
            return;
        }
        // Subtract the damage amount from the health of the object
        currentHealth -= damageAmount;
        if(damageNoise != null && damageTimer <= 0)
        {
            damageTimer = damageCooldown;
            Instantiate(damageNoise, transform.position, transform.rotation, null);
        }
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth + "/" + maxHealth + ".");
        updateDisplay();

        if (isPlayer) //EW: Added to make the player lose score after taking damage.
        {
            Debug.Log("HEALTH - Instance ID: " + this.GetInstanceID());
            if(style != null)
                style.DecreaseScore(damageAmount);
            DamageInfo info = new DamageInfo { source = sourcePosition, damage = damageAmount};
            PlayerTookDamage?.Invoke(info);
            if (PlayerTookDamage != null)
            {
                Debug.Log("HEALTH - The player taking damage event has been invoked.");
            }
            else
                Debug.Log("HEALTH - The player taking damage event is not invoking");
        }
        else //LB: adding an else to emit particles for just NPCs
        {
            if(bloodVFX != null && tickTimer <= 0)
            {
                //LB: Takes the percentage interpolates between min and max, ceils into a final integer. Emits the count of particles.
                float percentage = damageAmount / 100;
                if(damageAmount > 100)
                    percentage = 100;
                float particlesCount = Mathf.Lerp(minParticle, maxParticle, percentage);
                float speed = Mathf.Lerp(minSpeed, maxSpeed, percentage);
                int count = (int)Mathf.Ceil(particlesCount);
                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                ParticleSystem.MainModule main = bloodVFX.main;
                main.startSpeed = speed;
                bloodVFX.Emit(emitParams, count);
            }

            if((familyPhotoPromotion = GetComponent<P_FamilyPhoto>()) != null)
            {
                familyPhotoPromotion.SendHealing(damageAmount);
            }
        }

        // If the object has 0 or less current health
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // If a hit effect has been assigned
            if (hitEffect != null && tickTimer <= 0)
            {
                tickTimer = tick;
                // Play the hit effect for the object
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
        }
    }
    public void TakeNonLethalDamage(float damageAmount) //EW: Nonlethal damage added for possession gun, but might be useful elsewhere.
    {
        // Subtract the damage amount from the health of the object
        currentHealth -= damageAmount;
        if (damageNoise != null && damageTimer <= 0)
        {
            damageTimer = damageCooldown;
            Instantiate(damageNoise, transform.position, transform.rotation, null);
        }
        Debug.Log(gameObject.name + " took " + damageAmount + " nonlethal damage. Current Health: " + currentHealth + "/" + maxHealth + ".");
        updateDisplay();

        // If the object has 0 or less current health
        if (currentHealth <= 0)
        {
            currentHealth = 1;
            nonlethalDefeat = true;
        }
        else
        {
            // If a hit effect has been assigned
            if (hitEffect != null)
            {
                // Play the hit effect for the object
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
        }
    }

        // Applies a certain amount of healing to an object
        public void ReceiveHealing(float healingAmount)
    {
        // Add the healing amount to the object's current health
        currentHealth += healingAmount;
        Debug.Log(gameObject.name + " received " + healingAmount + " healing. Current Health: " + currentHealth + "/" + maxHealth + ".");
        updateDisplay();

        // If the object's current health is now greater than the max...
        if (currentHealth > maxHealth)
        {
            // Set current health to max health
            currentHealth = maxHealth;
        }
    }

    public void Die()
    {
        // If a death effect has been assigned
        if (deathEffect != null)
        {
            // Play the death effect for the object
            Instantiate(deathEffect, transform.position, transform.rotation, null);
        }

        // Destroy the game object
        Debug.Log(gameObject.name + " has died.");
        if (!isPlayer)
        {
            EnemyType enemy = GetComponent<EnemyType>();
            if (enemy != null && KillTracker.Instance != null)
            {
                KillTracker.Instance.RegisterKill(enemy.enemyId);
            }

            if(style != null)
                style.IncreaseScore(true, false); //EW: Added for the style gauge.
            playerLevel.AddEXP(EXPDrop);
            EnemyClass enmy = GetComponent<EnemyClass>();
            if (enmy != null)
            {
                enmy.Die();
            }

            if(bloodVFX != null)
            {
                bloodVFX.transform.SetParent(null, true);
                bloodVFX.transform.localScale = Vector3.one;
                ParticleSystem.MainModule main = bloodVFX.main;
                main.scalingMode = ParticleSystemScalingMode.Local;
                Destroy(bloodVFX.gameObject, 8f); 
            }



            // RL: Event for Achievement Manager
            GameEvent.OnEnemyKilled?.Invoke();
            Destroy(gameObject);
        }
        else if (isPlayer)
        {
            if (UIManager.instance.IsPaused)
            {
                Debug.Log("HEALTH - UNPAUSING THE GAME AS THE PLAYER DIED");
                UIManager.instance.TogglePause();
            }
            else
                Debug.Log("HEALTH - UI MANAGER INSTANCE IS PAUSED DOES NOT TOGGLE UN-PAUSE THE GAME");
            UIManager.instance.allowPause = false; //EW: Set this way to keep the player from pausing and unpausing after death because unpausing makes the main game UI show up.
            isDead = true;
            gameOverCanvas.SetActive(true);
            inGameCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("HEALTH - Ending Grade Script Activating");
            end = GameObject.FindAnyObjectByType<EndingGrade>();
            if (end != null && endScorePlaying != true) // added endScorePlaying variable to make sure the ending stuff only runs once
            {
                endScorePlaying = true;
                StartCoroutine(end.EndingGradeCoroutine());
                Debug.Log("HEALTH - END ISNOT NULL AND COROUTINE IS STARTING");
            }
            else
                Debug.Log("HEALTH - END IS NULL");
        }
    }

    // Allows you to set a new maximum health for an object while either maintaining their relative health or fully healing them.
    public void setMaxHealth(float newMaxHealth, bool healToNewMax)
    {
        if (newMaxHealth <= 0)
        {
            Debug.LogWarning("Health: setMaxHealth cannot be less than or equal to 0.");
            return;
        }

        // Storing old values for ratio calculation
        float oldMaxHealth = maxHealth;
        float healthRatio = (oldMaxHealth > 0) ? currentHealth / oldMaxHealth : 1f;

        maxHealth = newMaxHealth;

        // See if object should be fully healed
        if (healToNewMax)
        {
            currentHealth = maxHealth;
        }
        else
        {
            // Maintain relative health
            currentHealth = healthRatio * maxHealth;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        updateDisplay();
    }

    public void updateDisplay()
    {
        // Used if there is a TMPro Display assinged
        if (healthDisplayText != null)
        {
            healthDisplayText.text = string.Format(currentHealth + " / " + maxHealth);
        }

        // If there is a healthBar assigned
        if (healthBar != null)
        {
            healthBar.Activate();
            healthBar.SetHealth(currentHealth);
            healthBarActive = true;
        }
        else
        {
            Debug.Log("HEALTH - The Health Bar is null");
        }
    }

    #region MetaProgression
    private void OnEnable()
    {
        GameEvent.OnAchivementEarned += ApplyAchievementHealthIncreases;
    }
    public void ApplyAchievementHealthIncreases()
    {
        int healthUpAchievements = 0;

        // If an achievement's reward contains the text "+5% health", add it to the tally of health up achievements
        if (AchievementManager.Instance.database.achievements != null)
        {
            foreach (Achievement achievement in AchievementManager.Instance.database.achievements)
            {
                if (achievement.unlocked && achievement.reward.Contains("+5% Health"))
                {
                    healthUpAchievements++;
                }
            }
        }

        // Calculate bonus health
        float bonusHealth = healthUpAchievements * (initialMaxHealth * 0.05f);

        // Check if current max health is the same as the projected new max health
        // Done to prevent accidental heals provided when no health increasing achivements are earned
        if (maxHealth == initialMaxHealth + bonusHealth)
        {
            return;
        }

        if (GameObject.FindAnyObjectByType<HealthBarIndicator>() != null)
        {
            healthBar = GameObject.FindAnyObjectByType<HealthBarIndicator>().GetComponent<HealthBar>();
            Debug.Log("HEALTH - health bar is: " + healthBar);
        }

        // Set player's max health based on the amount of achievements
        setMaxHealth(initialMaxHealth + bonusHealth, true);
        healthBar.SetMaxHealth(maxHealth);
    }
    #endregion

    public bool HasDamageListeners()
    {
        return PlayerTookDamage != null;
    }
}
