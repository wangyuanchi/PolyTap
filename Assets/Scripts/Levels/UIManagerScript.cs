using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class UIManagerScript : MonoBehaviour
{
    public float beatMapStartTime;
    public float musicDuration;

    [Header("Audio")]
    [SerializeField] private GameObject AudioManager;
    [SerializeField] private GameObject musicObject;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite HeartEmpty;
    [SerializeField] private Sprite HeartFull;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject normalModeProgressBar;
    [SerializeField] private GameObject hardModeProgressBar;
    [SerializeField] private Slider musicSlider;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject restartSoonText;

    [Header("Others")]
    [SerializeField] private TMP_Text progressText;

    [Header("Input")]
    [SerializeField] private InputActionReference pauseActionReference;

    private float audioCompletedDuration;
    private float progressPercentage;

    private void OnEnable()
    {
        pauseActionReference.action.Enable();
        pauseActionReference.action.performed += onPause;
    }

    private void OnDisable()
    {
        pauseActionReference.action.performed -= onPause;
        pauseActionReference.action.Disable();
    }

    private void onPause(InputAction.CallbackContext context)
    {
        if (pauseUI.activeSelf)
        { ResumeScene(); }
        else
        { PauseScene(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load and set the music volume
        musicSlider.value = PlayerPrefs.GetFloat("Music Volume");
        SetMusicVolume();

        SetDifficulty();
        SetProgressBar();
    }

    // Update is called once per frame
    void Update()
    {
        // Do not increase progress if game is over
        if (!gameOverUI.activeSelf)
        {
            updateProgressPercentage();

            if (progressPercentage < 100f)
            {
                progressText.text = $"{progressPercentage}%";
            }
            else
            {
                // Make sure progressPercentage is at exactly 100
                progressPercentage = 100f;
                progressText.text = $"{progressPercentage}%";
                StartCoroutine(GameOver(true));
            }
        }
    }

    private void SetDifficulty()
    {
        if (PlayerPrefs.GetString("Hard Mode") == "false")
        {
            health = 3;
        }
        else
        {
            health = 1;
            hearts[1].sprite = HeartEmpty;
            hearts[2].sprite = HeartEmpty;
        }
    }

    // Load and set progress bars
    private void SetProgressBar()
    {
        string levelName = SceneManager.GetActiveScene().name;
        float normalModeHighScore = PlayerPrefs.GetFloat($"{levelName}-N-HS");
        float hardModeHighScore = PlayerPrefs.GetFloat($"{levelName}-H-HS");

        // Set progress bar fill
        normalModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = normalModeHighScore / 100;
        hardModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = hardModeHighScore / 100;

        // Set progress text
        normalModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = normalModeHighScore.ToString() + "%";
        hardModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = hardModeHighScore.ToString() + "%";
    }

    private void updateProgressPercentage()
    {
        audioCompletedDuration = Time.time - beatMapStartTime;
        progressPercentage = (float) Math.Round(audioCompletedDuration / musicDuration * 100f, 2);
    }

    public void DecreaseHealth()
    {
        health--;

        // Change sprite of hearts based on health
        for (int heart = 0; heart < hearts.Count(); heart++)
        {
            if (heart < health)
            {
                hearts[heart].sprite = HeartFull;
            }
            else
            {
                hearts[heart].sprite = HeartEmpty;
            }
        }

        // End the game if no health is left
        if (health == 0)
        {
            StartCoroutine(GameOver(false));
        }
    }

    // When game over happens, the progress is stopped and the audio is paused, but the beatmap still plays for aesthetics
    private IEnumerator GameOver(bool levelComplete)
    {
        SetHighScore();
        SetProgressBar();

        if (levelComplete) 
        {
            gameOverText.text = "Level Complete!";
            restartSoonText.SetActive(false);
        }
        else
        {
            gameOverText.text = "Game Over!" + Environment.NewLine + $"Progress: {progressPercentage}%";
        }

        AudioManager.GetComponent<AudioManagerScript>().StopMusic();
        gameOverUI.SetActive(true);

        // Pause for 3 seconds before restarting the scene
        if (!levelComplete)
        {
            yield return new WaitForSeconds(3);
            RestartScene();
        }
    }

    // Setting of high score in player prefs
    private void SetHighScore()
    {
        string key;
        
        if (PlayerPrefs.GetString("Hard Mode") == "false")
        {
            key = SceneManager.GetActiveScene().name + "-N-HS";
        }
        else
        {
            key = SceneManager.GetActiveScene().name + "-H-HS";
        }

        float highScore = PlayerPrefs.GetFloat(key);
        if (highScore < progressPercentage)
        {
            PlayerPrefs.SetFloat(key, progressPercentage);
        }
    }

    private void PauseScene()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        AudioManager.GetComponent<AudioManagerScript>().PauseMusic();
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResumeScene()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;

        // Prevent clash where audio resumes if user pauses then unpauses after the game has ended
        if (!gameOverUI.activeSelf)
        { 
            AudioManager.GetComponent<AudioManagerScript>().ResumeMusic();
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(sceneName); 
    }

    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 25);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }
}
