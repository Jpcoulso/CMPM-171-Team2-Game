using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    private Button button;
    [SerializeField] private bool mapButton = false;
    [SerializeField] private int levelIndex;

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
            GameManager.Instance.LoadScene(sceneName);
        }
    }
}