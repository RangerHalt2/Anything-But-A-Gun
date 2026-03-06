using System;

public static class GameEvent
{
    public static Action OnAchivementEarned;

    public static Action<string> OnLevelCompleted; // Event to be triggered when a level is completed
    public static Action OnEnemyKilled; // Event to be triggered when an enemy is killed
    public static Action OnWeaponModified; // Event to be triggered when a weapon is modified
    public static Action StyleMaxxed; // Event to be triggered when the player maxes out the style meter
}
