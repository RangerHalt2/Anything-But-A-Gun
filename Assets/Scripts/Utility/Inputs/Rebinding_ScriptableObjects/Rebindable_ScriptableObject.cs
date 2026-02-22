using UnityEngine;

[CreateAssetMenu(fileName = "KeybindData", menuName = "Input/Keybind Data")]
public class Rebindable_ScriptableObject : ScriptableObject
{
    public InputManager.RebindableAction action;
    public int bindingIndex = 0;
}
