using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public string onePlayerScene;
    public string twoPlayerScene;

    void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OnePlayer(string difficulty)
    {
        GameManager.onePlayer = true;
        GameManager.difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), difficulty);
        LoadScene(onePlayerScene);
    }

    public void TwoPlayers()
    {
        GameManager.onePlayer = false;
        LoadScene(twoPlayerScene);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
