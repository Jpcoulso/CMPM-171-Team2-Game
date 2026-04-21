using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    
    public static SelectionManager Instance // Singlton design, we will have exactly one instance of this class that other objects can talk to
    {
        get;
        private set;
    }
    
    public CharacterSelector currentlySelected; // holds a reference to the currently selected character

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
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnRightClick();
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
    private void OnRightClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPosition.z = 0; // Set z to 0 for 2D

        Debug.Log("Right Click at: " + mousePosition);
        Debug.Log("World Position: " + worldPosition);

        // Implement right-click logic here
        UnitController unitController = currentlySelected.GetComponent<UnitController>();
        if (unitController != null)
        {
            unitController.Move(worldPosition);
        }
    }
    public void SelectCharacter(CharacterSelector newSelection) // used to assign a character to currentlySelected
    {
        if (newSelection == currentlySelected)
        {
            return;
        }
        currentlySelected?.Deselect();
        currentlySelected = newSelection;
        currentlySelected.Select();
    }
}
