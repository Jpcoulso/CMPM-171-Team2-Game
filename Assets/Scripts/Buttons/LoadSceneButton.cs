using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not set on LoadSceneButton!");
            SceneManager.LoadScene("CombatScene"); // Default to CombatScene if not set
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}