using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    private Button button;
    [SerializeField] private bool mapButton = false;
    [SerializeField] private int levelIndex;
    [SerializeField] private AudioManager.MusicType musicType;

    void Awake()
    {
        button = GetComponent<Button>();
        if (mapButton)
        {
            button.interactable = GameManager.Instance.IsLevelUnlocked(levelIndex);
        }
    }
    public void LoadSceneByName(string sceneName)
    {
        if (!mapButton || GameManager.Instance.IsLevelUnlocked(levelIndex))
        {
            if (musicType != AudioManager.MusicType.None) { AudioManager.Instance.FadeToMusic(musicType); }
            GameManager.Instance.LoadScene(sceneName);
        }
    }
}