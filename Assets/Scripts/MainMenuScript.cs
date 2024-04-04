using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    { SceneManager.LoadSceneAsync(sceneName); }

    public void QuitGame()
    { Application.Quit(); }

    // Called once per lifetime
    void Awake()
    {
        // Setting of player preferences
        PlayerPrefs.SetFloat("Level 1 High Score", PlayerPrefs.GetFloat("Level 1 High Score", 0f));
    }
}
