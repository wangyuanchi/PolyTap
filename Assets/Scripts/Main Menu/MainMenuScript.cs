using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject sceneTransition;
    [SerializeField] private AudioMixer audioMixer;

    // Called once per lifetime
    void Awake()
    {
        // Uncomment, reset and recomment player prefs before building the game!        
        // PlayerPrefs.DeleteAll();

        // Setting of player preferences
        PlayerPrefs.SetFloat("Music Volume", PlayerPrefs.GetFloat("Music Volume", 0.5f));
        PlayerPrefs.SetFloat("SFX Volume", PlayerPrefs.GetFloat("SFX Volume", 0.5f));
        PlayerPrefs.SetString("Lobby Music", PlayerPrefs.GetString("Lobby Music", "true"));
        PlayerPrefs.SetInt("Note Speed", PlayerPrefs.GetInt("Note Speed", 5));
        PlayerPrefs.SetString("Mode", PlayerPrefs.GetString("Mode", "N"));

        PlayerPrefs.SetFloat("L1-N-HS", PlayerPrefs.GetFloat("L1-N-HS", 0f));
        PlayerPrefs.SetFloat("L1-H-HS", PlayerPrefs.GetFloat("L1-H-HS", 0f));
        PlayerPrefs.SetInt("L1-N-TA", PlayerPrefs.GetInt("L1-N-TA", 0));
        PlayerPrefs.SetInt("L1-H-TA", PlayerPrefs.GetInt("L1-H-TA", 0));
    }

    // Start is called before the first frame update
    void Start()
    { 
        LoadAudioVolume();
    }

    private void LoadAudioVolume()
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(PlayerPrefs.GetFloat("Music Volume")) * 25);
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(PlayerPrefs.GetFloat("SFX Volume")) * 25);
    }

    public void TransitionToScene(string levelName)
    {
        sceneTransition.GetComponent<SceneTransitionScript>().TransitionToScene(levelName);
    }

    public void QuitGame()
    { Application.Quit(); }
}
