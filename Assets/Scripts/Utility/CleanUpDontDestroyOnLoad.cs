using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanupDontDestroyOnLoad : MonoBehaviour
{
    public static void DestroyAllDontDestroyOnLoad()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.buildIndex == -1 && obj.transform.parent == null)
            {
                Debug.Log("Destroying: " + obj.name);
                Destroy(obj);
            }
        }
    }

    public static void DestroyAllDontDestroyOnLoad(GameObject exclude)
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.scene.buildIndex == -1 && obj.transform.parent == null)
            {
                if (exclude != null && obj == exclude) continue;
                Debug.Log("Destroying: " + obj.name);
                Destroy(obj);
            }
        }
    }
}