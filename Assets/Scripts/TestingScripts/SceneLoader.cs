using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingBar;
    public void LoadScene(int levelIndex)
    {
        StartCoroutine(LoadSceneAsynchonously(levelIndex));
    }


    IEnumerator LoadSceneAsynchonously(int levelIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        loadingScreen.SetActive(true);
        while (operation != null)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }
    }
}
