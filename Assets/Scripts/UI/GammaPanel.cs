using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class GammaPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject panel;
    public InputAction pauseAction;
    [SerializeField] private InputActionAsset UIControls;

    void Start()
    {
        var uiMap = UIControls.FindActionMap("UI", true);
        pauseAction = uiMap.FindAction("Pause", true);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            if (panel != null)
            {
                bool isActive = panel.activeSelf;
                panel.SetActive(!isActive);
            }

        }
    }
}