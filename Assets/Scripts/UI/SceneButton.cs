using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }
    public void LoadSceneByName(string sceneName)
    {
        GameManager.Instance.LoadScene(sceneName);
    }
}