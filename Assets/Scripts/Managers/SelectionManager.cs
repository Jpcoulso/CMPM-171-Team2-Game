using UnityEngine;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    
    public static SelectionManager Instance // Singlton design, we will have exactly one instance of this class that other objects can talk to
    {
        get;
        private set;
    }
    
    private List<CharacterSelector> selectedCharacters = new List<CharacterSelector>();
    public IReadOnlyList<CharacterSelector> SelectedCharacters => selectedCharacters;
    public CharacterSelector currentlySelected => selectedCharacters.Count > 0 ? selectedCharacters[0] : null; // returns the first character in the list or null if list is empty

    private void Awake() // ensures there is only ever one instance of SelectionManager
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    
    // private void CheckForCharacterClick()
    // {
    //     Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); // stores location of mouse when it was clicked
    //     Debug.Log("CheckForCharacterClick at world point: " + worldPoint);
    //     RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero); // creates a raycast at mouse location (vector2.zero as the direction means it has no direction)
    //     if (hit.collider == null)
    //     {
    //         Debug.Log("Raycast hit nothing — no collider found at click position.");
    //         return;
    //     }
    //     Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
    //     CharacterSelector clickedCharacter = hit.collider.GetComponent<CharacterSelector>();
    //     if (clickedCharacter != null)
    //     {
    //         SelectCharacter(clickedCharacter);
    //     }
    //     else
    //     {
    //         Debug.Log("Hit object has no CharacterSelector component.");
    //     }
    // }
    // private void OnRightClick()
    // {
    //     Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    //     worldPosition.z = 0; // Set z to 0 for 2D

    //     // Implement right-click logic here
    //     if (currentlySelected == null)
    //     {
    //         Debug.Log("No unit selected — left-click a unit first.");
    //         return;
    //     }
    //     UnitController unitController = currentlySelected.GetComponent<UnitController>();
    //     if (unitController != null)
    //     {
    //         unitController.Move(worldPosition);
    //     }
    // }
    public void SelectCharacter(CharacterSelector newSelection) // used to assign a character to currentlySelected
    {
        foreach (var c in selectedCharacters)
        {
            c.Deselect();
        }
        selectedCharacters.Clear();
        selectedCharacters.Add(newSelection);
        newSelection.Select();
    }
    public void AddToSelection(CharacterSelector newSelection)
    {
        if (!selectedCharacters.Contains(newSelection))
        {
            selectedCharacters.Add(newSelection);
            newSelection.Select();
        }
    }
    public void SelectCharacter() // used to deselect currently selected character
    {
        foreach (var c in selectedCharacters)
        {
            c.Deselect();
        }
        selectedCharacters.Clear();
    }
}
