using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider LoadingBarSlider;

    private void Start()
    {
        if (LoadingScreen == null && GameObject.FindAnyObjectByType<LoadingIndicator>() != null)
            LoadingScreen = GameObject.FindAnyObjectByType<LoadingIndicator>().gameObject;
        if(LoadingBarSlider == null && GameObject.FindAnyObjectByType<LoadingBarIndicator>() != null)
        {
            LoadingBarSlider = GameObject.FindAnyObjectByType<LoadingBarIndicator>().GetComponent<Slider>();
        }

    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        string previousScene = SceneManager.GetActiveScene().name;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            
            LoadingBarSlider.value = Mathf.Lerp(LoadingBarSlider.value, targetProgress, Time.deltaTime * 5f);

            
            if (operation.progress >= 0.9f)
            {
                LoadingBarSlider.value = 1f; 
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        if (transform.root.gameObject.GetComponent<UIManager>() != null && previousScene != "Level Gen 5")
        {
            Debug.Log("LOADING SCENE - Previous Scene" + previousScene);
            UIManager manager = transform.root.gameObject.GetComponent<UIManager>();
            manager.GoToPage(0);
            LoadingScreen.SetActive(false);
        }
        else
        {
            LoadingScreen.SetActive(false);
        }
    }
}
