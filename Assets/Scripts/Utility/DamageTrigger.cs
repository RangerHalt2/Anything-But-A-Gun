// Created by: Ryan Lupoli
// Deals damage when an object enters a trigger
using UnityEngine;
using System.Collections.Generic;

public class DamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("The amount of damage done by the damage trigger.")]
    [SerializeField] private float damage;
    [SerializeField] private float distanceTolerance = 1f;

    [Header("Team Settings")]
    [Tooltip("A list of all team IDs which should be effected by the damage trigger")]
    [SerializeField] private List<int> targetTeamIDs;

    private float cooldown = 1f;
    private float timer = 0f;

    private void Start()
    {
        distanceTolerance = 6f;
    }

    // Runs when something enters the trigger
    private void OnTriggerStay(Collider other)
    {
        Health health = other.gameObject.GetComponentInParent<Health>();

        // If the object has a health component...
        if (health != null && IsInRange(health.transform) && timer <= 0)
        {
            timer = cooldown;
            // Get the object's teamID
            int otherTeamID = health.teamID;

            // Check if the list contains the object's teamID
            if (targetTeamIDs.Contains(otherTeamID))
            {
                // Apply damage
                health.TakeDamage(damage, this.gameObject.transform);
            }
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    private bool IsInRange(Transform objectInQuestion)
    {
        bool ret = false;

        if (objectInQuestion != null)
        {
            float distance = Vector3.Distance(objectInQuestion.position, this.transform.position);
            if (distance <= distanceTolerance)
            {
                ret = true;
            }
        }
        return ret;
    }

}
