using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    
    public static SelectionManager Instance // Singlton design, we will have exactly one instance of this class that other objects can talk to
    {
        get;
        private set;
    }
    
    private CharacterSelector _currentlySelected; // holds a reference to the currently selected character

    private void Awake() // ensures there is only ever one instance of SelectionManager
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForCharacterClick();
        }
    }
    private void CheckForCharacterClick()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // stores location of mouse when it was clicked
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero); // creates a raycast at mouse location (vector2.zero as the direction means it has no direction)
        if (hit.collider == null)
        {
            return;
        }
        CharacterSelector clickedCharacter = hit.collider.GetComponent<CharacterSelector>();
        if (clickedCharacter != null)
        {
            SelectCharacter(clickedCharacter);
        }
    }
    public void SelectCharacter(CharacterSelector newSelection) // used to assign a character to _currentlySelected
    {
        if (newSelection == _currentlySelected)
        {
            return;
        }
        _currentlySelected?.Deselect();
        _currentlySelected = newSelection;
        _currentlySelected.Select();
    }
}
