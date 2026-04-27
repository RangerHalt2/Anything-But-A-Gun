using UnityEngine;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    public GameObject defaultSelectedPauseObject;

    public void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
    }

    public void SetSelected(GameObject selectedObject)
    {
        EventSystem.current.SetSelectedGameObject(selectedObject);
    }
}
