// Created by: Ryan Lupoli
// Deals damage when an object enters a trigger
using UnityEngine;
using System.Collections.Generic;

public class DamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("The amount of damage done by the damage trigger.")]
    [SerializeField] private float damage;

    [Header("Team Settings")]
    [Tooltip("A list of all team IDs which should be effected by the damage trigger")]
    [SerializeField] private List<int> targetTeamIDs;

    // Runs when something enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        Health health = other.gameObject.GetComponentInParent<Health>();

        // If the object has a health component...
        if (health != null)
        {
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
}
