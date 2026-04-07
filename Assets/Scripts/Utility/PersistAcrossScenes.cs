using System.Collections.Generic;
using UnityEngine;

public class PersistAcrossScenes : MonoBehaviour
{
    private static List<string> persistedObjects = new List<string>();

    private void Awake()
    {
        if (persistedObjects.Contains(gameObject.name))
        {
            Destroy(gameObject);
            return;
        }

        persistedObjects.Add(gameObject.name);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        persistedObjects.Remove(gameObject.name);
    }

}
