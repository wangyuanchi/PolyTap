using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManagerScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    { 
        SceneManager.LoadSceneAsync(sceneName); 
    }
    
}

