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
        UIManager uiManager = GameObject.FindAnyObjectByType<UIManager>();
        uiManager.allowPause = false;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log("LOADING SCENE - LoadingScreen before checks: " + LoadingScreen + " | is null: " + (LoadingScreen == null) + " | ReferenceEquals null: " + ReferenceEquals(LoadingScreen, null));

        if (LoadingScreen == null)
        {
            LoadingIndicator indicator = GameObject.FindAnyObjectByType<LoadingIndicator>(FindObjectsInactive.Include);
            Debug.Log("LOADING SCENE - Indicator found: " + indicator);
            if (indicator != null)
                LoadingScreen = indicator.gameObject;
        }

        if (LoadingBarSlider == null)
        {
            LoadingBarIndicator barIndicator = GameObject.FindAnyObjectByType<LoadingBarIndicator>(FindObjectsInactive.Include);
            if (barIndicator != null)
                LoadingBarSlider = barIndicator.GetComponent<Slider>();
        }

        string previousScene = SceneManager.GetActiveScene().name;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;



        Debug.Log("LOADING SCENE - LoadingScreen: " + LoadingScreen + " | Slider: " + LoadingBarSlider);
        LoadingScreen.SetActive(true);
        Debug.Log("LOADING SCENE - loading scene was set to active: " + LoadingScreen.activeInHierarchy);
        operation.allowSceneActivation = false;
        bool readyToLoad = false;
        while (!operation.isDone)
        {
            Debug.Log("LOADING SCENE - iterating through the operation.");
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            if (LoadingBarSlider != null)
                LoadingBarSlider.value = Mathf.Lerp(LoadingBarSlider.value, targetProgress, Time.deltaTime * 5f);

            
            if (operation.progress >= 0.9f && !readyToLoad)
            {
                Debug.Log("LOADING SCENE - operation should be done");
                if (LoadingBarSlider != null)
                    LoadingBarSlider.value = 1f;
                readyToLoad = true;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        if (transform.root.gameObject.GetComponent<UIManager>() != null && previousScene != "Level Gen 5")
        {
            Debug.Log("LOADING SCENE - Previous Scene" + previousScene);
            UIManager manager = transform.root.gameObject.GetComponent<UIManager>();
            manager.GoToPage(0);
            if (LoadingBarSlider != null)
                LoadingScreen.SetActive(false);
        }
        else
        {
            if (LoadingBarSlider != null)
                LoadingScreen.SetActive(false);
        }
    }
}
