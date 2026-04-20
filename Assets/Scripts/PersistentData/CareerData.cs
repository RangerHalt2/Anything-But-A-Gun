// Created By: Ryan Lupoli
using System;
using System.Collections.Generic;

[Serializable]
public class CareerData
{
    public int enemiesKilled = 0; // The total number of enemies killed during the run
    public int levelsCompleted = 0; // The total number of levels the player completed

    // Tracks what weapon the player has collected across all runs
    public List<string> weaponsCollected = new List<string>();
}
