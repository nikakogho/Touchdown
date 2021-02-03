using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
    public GameObject pauseUI;

    public static bool Paused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown("p")) TogglePause();
    }

    public void TogglePause()
    {
        Paused = !Paused;
        pauseUI.SetActive(Paused);
        if (Paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
