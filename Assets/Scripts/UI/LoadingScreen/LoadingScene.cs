using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider LoadingBarSlider;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
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

        LoadingScreen.SetActive(false);
    }
}
