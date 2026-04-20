// Created By: Ryan Lupoli
using System;
using System.Collections.Generic;

[Serializable]
public class RunData
{
    public int enemiesKilled = 0; // The total number of enemies killed during the run
    public int levelsCompleted = 0; // The total number of levels the player completed

    // Tracks what weapon the player has collected during the current run
    public List<string> weaponsCollected = new List<string>();
}
