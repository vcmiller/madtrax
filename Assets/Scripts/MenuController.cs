using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public void LoadLevel(string level) {
        SceneManager.LoadScene(level, LoadSceneMode.Single);
    }

    public void Quit() {
        Application.Quit();
    }
}
