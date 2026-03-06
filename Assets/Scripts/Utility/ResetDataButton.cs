// Created By: Ryan Lupoli
// Simple helper function that allows for a Unity Button to reset persistent data
using UnityEngine;

public class ResetDataButton : MonoBehaviour
{
    // Called by the UI Button
    public void ResetGameData()
    {
        if (ResetData.Instance == null)
        {
            Debug.LogWarning("ResetDataButton: ResetData instance not found in scene.");
            return;
        }

        ResetData.Instance.ResetAllData();
       // Debug.Log("ResetDataButton: ResetAllData() called.");
    }
}
