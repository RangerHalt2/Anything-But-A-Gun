using UnityEngine;
using UnityEngine.Rendering;

public class HubLevelSender : MonoBehaviour, IInteractable
{
    [SerializeField] private string[] denials = new string[0];
    public string[] denyText { get { return denials; } set { denials = value; } }

    [SerializeField] private string LevelName;
    private SceneController sceneController;
    [SerializeField] private Vector3 nextScenePos;

    public bool canInteract { get; set; } = true;

    public void Interact()
    {
        if (sceneController != null && canInteract)
        {
            // Move player to the specified position in the next scene
            PlayerController player = GameObject.FindAnyObjectByType<PlayerController>();
            PlayerController controller = GameObject.FindAnyObjectByType<PlayerController>();
            controller.isSpawned = false;
            player.transform.position = nextScenePos;
            // Go to the next scene
            sceneController.GoToScene(LevelName);
        }
        else
        {
            Debug.LogWarning("Win Event: Scene Controller could not be found in current scene! Cannot load next scene!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            pc.CheckInteract();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            pc.CheckInteract();
            pc.LeftInteract();
        }
    }

    private void Start()
    {
        sceneController = GameObject.FindAnyObjectByType<SceneController>();
    }
}
