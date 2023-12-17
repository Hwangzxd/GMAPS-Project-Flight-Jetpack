using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    public void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        //Time.timeScale = 0f;

        //AudioSource[] audios = FindObjectsOfType<AudioSource>();

        //foreach (AudioSource a in audios)
        //{
        //    a.Pause();
        //}

        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        //Time.timeScale = 1f;

        //AudioSource[] audios = FindObjectsOfType<AudioSource>();

        //foreach (AudioSource a in audios)
        //{
        //    a.Play();
        //}

        isPaused = false;
    }

    public void Back()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
