using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName ="Level Data")]
public class LevelDataScriptableObject : ScriptableObject
{
    [SerializeField] public AudioClip levelMusic;
    [SerializeField] public Sprite background;
}
