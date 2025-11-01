using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseballBat_V2 : MonoBehaviour, IWeapon
{
    [Header("Weapon Settings")]
    public int level { get; set; } = 1;
    [Tooltip("The base amount of damage the weapon deals before accounting for levels.")]
    [SerializeField] private float baseDamage = 15;
    [Tooltip("The amount of extra damage added for every level on the weapon.")]
    [SerializeField] private float levelDamage = 5f;
    public float cumulativeDamage;
    private WeaponLevel weaponLevel;

    [Header("Swing Settings")]
    [Tooltip("The amount of time (in Seconds) between swings.")]
    [SerializeField] private float swingCooldown = 1f;
    [Tooltip("The amount of time (in Seconds) the bat's hitbox will be active for.")]
    [SerializeField] private float swingDuration = 0.3f;
    private bool canSwing = true;
    private bool firstHit = true;

    private AmmoManager ammoManager;

    [Header("References")]
    [Tooltip("Reference to the bat's hitbox. Should be a child of the weapon.")]
    [SerializeField] private GameObject hitbox;
    [SerializeField] private GameObject hitEffect;
    

    void Start()
    {
        // If a hitbox is assigned, deactivate it
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }

        // Get reference to the weapon level and ammo Manager
        weaponLevel = GetComponent<WeaponLevel>();
        ammoManager = GetComponent<AmmoManager>();
    }

    public void Shoot()
    {
        // If player can swing...
        if (canSwing)
        {
            CalculateLevelDamage();
            // Start the swing coroutine
            StartCoroutine(Swing());
        }
    }

    private IEnumerator Swing()
    {
        canSwing = false;

        // Enable hitbox
        if (hitbox != null)
        {
            hitbox.SetActive(true);
        }

        // Wait for the duration of the swing
        yield return new WaitForSeconds(swingDuration);

        // Disable hitbox
        if (hitbox != null)
        {
            hitbox.SetActive(false);
        }

        // Wait for the remaingin time the swing is on cooldown
        yield return new WaitForSeconds(swingCooldown - swingDuration);

        // Allow player to swing again
        canSwing = true;
        firstHit = true;
    }

    public void Reload()
    {
    }

    public void RegisterHit()
    {
        // Only allow this method to be called once per swing
        if (firstHit)
        {
            firstHit = false;
            // Decreas Ammo in ammo Manager
            ammoManager.Fire();

            // Play hit effect, if assigned
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation, null);
            }
        }
    }

    public void CalculateLevelDamage()
    {
        cumulativeDamage = baseDamage + levelDamage * (weaponLevel.Level - 1);
        //Debug.Log("Weapon: Cumulative Damage " + cumulativeDamage);
    }

    public float GetCumulativeDamage()
    {
        return cumulativeDamage;
    }
}