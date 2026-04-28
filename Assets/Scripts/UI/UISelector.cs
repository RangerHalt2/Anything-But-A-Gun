using UnityEngine;
using UnityEngine.EventSystems;

public class UISelector : MonoBehaviour
{
    public GameObject defaultSelectedPauseObject;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameObject.FindAnyObjectByType<InputManager>();
    }

    public void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedPauseObject);
    }

    public void SetSelected(GameObject selectedObject)
    {
        EventSystem.current.SetSelectedGameObject(selectedObject);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && inputManager.ControllerLast)
        {
            GameObject obj = GameObject.Find("B_Audio");
            if(obj == null)
            {
                obj = GameObject.Find("B_HowToPlay");
            }
            if(obj != null)
                EventSystem.current.SetSelectedGameObject(obj);
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            GameObject obj = GameObject.Find("B_HowToPlay");
        }
    }

}
