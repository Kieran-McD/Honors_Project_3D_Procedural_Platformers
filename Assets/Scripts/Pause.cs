using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{


    public GameObject pauseMenu;
    public Slider sensitivtySlider;

    private void Start()
    {
        pauseMenu.SetActive(false);
        sensitivtySlider.value = PlayerPrefs.GetFloat("Sensitivity");
    }

    public void Update()
    {
       
    }

    public void SensititvityChange(float change)
    {
        PlayerPrefs.SetFloat("Sensitivity", sensitivtySlider.value);
    }

    public void PauseInput()
    {
        if (pauseMenu.activeSelf)
        {
            UnPauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }
}
