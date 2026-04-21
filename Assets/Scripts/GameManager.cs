using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton pattern — any script can access GameManager.Instance
    public static GameManager Instance { get; private set; }

    // The currently selected character (null if none)
    public CharacterController2D SelectedCharacter { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Call this to select a character. Deselects the previous one first.
    public void SelectCharacter(CharacterController2D character)
    {
        // Deselect whoever was selected before
        if (SelectedCharacter != null)
            SelectedCharacter.OnDeselected();

        SelectedCharacter = character;

        if (SelectedCharacter != null)
            SelectedCharacter.OnSelected();
    }

    // Call this to deselect everyone (e.g. click on empty space)
    public void DeselectAll()
    {
        if (SelectedCharacter != null)
            SelectedCharacter.OnDeselected();
        SelectedCharacter = null;
    }
}