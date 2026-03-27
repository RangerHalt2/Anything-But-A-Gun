using System;

public static class GameEvent
{
    public static Action RunStarted; // Event to be called whenever the player starts a new run
    public static Action RunEnded; // Event to be called whenever the player ends a run

    public static Action<UnityEngine.GameObject> OnWeaponPickup; // Event to be triggered whenever a weapon is picked up by the player

    public static Action OnAchivementEarned; // Event to be triggered whenever an achievement is unlocked

    public static Action<string> OnLevelCompleted; // Event to be triggered when a level is completed
    public static Action OnEnemyKilled; // Event to be triggered when an enemy is killed
    public static Action OnWeaponModified; // Event to be triggered when a weapon is modified
    public static Action StyleMaxxed; // Event to be triggered when the player maxes out the style meter
}
