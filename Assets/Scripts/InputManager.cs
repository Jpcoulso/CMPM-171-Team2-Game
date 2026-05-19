using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using NUnit.Framework.Internal;

public class InputManager : MonoBehaviour
{
    //[SerializeField] private InputActionAsset actions;
    [SerializeField] private ClickIndicator clickIndicatorPrefab; // Prefab for the click indicator
    private InputAction RightClick;
    private InputAction LeftClick;
    private InputAction Stop;

    public static InputManager Instance
    {
        get;
        private set;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        Instance = this;
    }
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftClick();
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnRightClick();
        }

        // Ability hotkeys
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Q ability activated");
            TryUseAbility(0);
        }
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("W ability activated");
            TryUseAbility(1);
        }
    }

    void TryUseAbility(int slotIndex)
    {
        if (SelectionManager.Instance.currentlySelected == null)
        {
            Debug.Log("Ability " + slotIndex + " pressed but no unit selected.");
            return;
        }

        Hero selectedHero = SelectionManager.Instance.currentlySelected.GetComponent<Hero>();
        if (selectedHero == null)
        {
            Debug.Log("Selected unit is not a Hero.");
            return;
        }

        selectedHero.UseAbility(slotIndex);
    }

    void OnRightClick()
    {
        // If no unit is currently selected, ignore right-clicks
        if (SelectionManager.Instance.currentlySelected == null)
        {
            Debug.Log("Right-clicked with no unit selected.");
            return;
        }
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPosition.z = 0; // Set z to 0 for 2D
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Check if we right-clicked on an enemy
        if (hit.collider != null && hit.collider.gameObject.GetComponent<Enemy>() != null)
        {
            Debug.Log("Right-clicked on an enemy.");
            //float range = 2f; // TEMP attack range ****************************COMMENTED OUT TO TEST STATE TRANSITION FROM CHASING TO ATTACKING
            Vector3 enemyPosition = hit.collider.transform.position;
            Vector3 unitPosition = SelectionManager.Instance.currentlySelected.transform.position;
            
            // Identify the currently selected Hero and the clicked enemy, set the enemy as the hero's target so that hero will aggro enemy
            Hero selectedHero = SelectionManager.Instance.currentlySelected.gameObject.GetComponent<Hero>();
            Enemy clickedEnemy = hit.collider.gameObject.GetComponent<Enemy>();
            selectedHero.SetTarget(clickedEnemy);
            // once we have a target for selected hero the state machine should take over and initiate movement and combat

            /*
            Vector3 directionToEnemy = (enemyPosition - unitPosition).normalized;
            Vector3 destination = enemyPosition - directionToEnemy; //* range;****************************COMMENTED OUT TO TEST STATE TRANSITION FROM CHASING TO ATTACKING
            selectedHero.SetDestination(destination); // feed distination to character.cs so that character can use it for movement and range calculations 
            */

            SpawnIndicator(worldPosition, true); // Spawn a red indicator for enemies
            // Move the selected unit into attack range
            if (SelectionManager.Instance == null || SelectionManager.Instance.currentlySelected == null)
            {
                Debug.Log("No unit selected — left-click a unit first.");
                return;
            }
            //************************************************************************************************************************  
            // moved into hero/enemy classes, hero still uses unitController, enemy uses its own move function
            /*
            UnitController unitController = SelectionManager.Instance.currentlySelected.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.Move(destination);
            }
            */
            //************************************************************************************************************************
        }
        // If not clicking on an enemy, move to the location as normal
        else
        {
            Hero selectedHero = SelectionManager.Instance.currentlySelected.gameObject.GetComponent<Hero>();
            selectedHero.SetDestination(worldPosition);
            SpawnIndicator(worldPosition, false); // Spawn a green indicator for allies
            /*
            UnitController unitController = SelectionManager.Instance.currentlySelected.GetComponent<UnitController>();
            if (unitController != null)
            {
                SpawnIndicator(worldPosition, false); // Spawn a green indicator for allies
                unitController.Move(worldPosition);
            }
            */
        }
    }
    void OnLeftClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Debug.Log("Left Click at: " + mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if we hit anything
        if (hit.collider == null)
        {
            Debug.Log("Left Click hit nothing.");
            SelectionManager.Instance.SelectCharacter(); // Deselect if we click on empty space
            return;
        }
        Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
        CharacterSelector clickedCharacter = hit.collider.GetComponent<CharacterSelector>();
        if (clickedCharacter != null)
        {
            SelectionManager.Instance.SelectCharacter(clickedCharacter);
        }
        else
        {
            Debug.Log("Hit object has no CharacterSelector component.");
        }
    }

    private void SpawnIndicator(Vector3 position, bool isEnemy)
    {
        ClickIndicator indicator = Instantiate(clickIndicatorPrefab, position, Quaternion.identity);
        indicator.SetColor(isEnemy); // Set color based on whether it's an enemy or not
    }
}
