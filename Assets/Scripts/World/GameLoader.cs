using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {
    public string menu;
    public float count = 4;

    void Update()
    {
        count -= Time.deltaTime;
        if (count <= 0) SceneManager.LoadScene(menu);
    }
}
