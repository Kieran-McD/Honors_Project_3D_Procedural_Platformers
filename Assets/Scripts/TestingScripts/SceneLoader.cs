using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public GameObject loadingScreen;
    public Slider loadingBar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

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
