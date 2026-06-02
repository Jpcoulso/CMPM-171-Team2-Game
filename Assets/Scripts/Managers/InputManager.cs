using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    //[SerializeField] private InputActionAsset actions;
    [SerializeField] private ClickIndicator clickIndicatorPrefab;
    private ClickIndicator clickIndicator;
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
        clickIndicator = Instantiate(clickIndicatorPrefab);
    }
    void Update()
    {
        // Only ignore world clicks that land on the HUD panel itself, so clicking
        // an ability button doesn't also deselect the hero behind it. (We check the
        // HUD rect specifically rather than all UI, so units stay clickable.)
        Vector2 mousePos = Mouse.current.position.ReadValue();
        bool pointerOverHud = TeamHUD.Instance != null
            && TeamHUD.Instance.IsPointerOverPanel(mousePos);

        if (!pointerOverHud && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftClick();
        }
        if (!pointerOverHud && Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnRightClick();
        }

        // Number keys 1-4 to select units
        if (Keyboard.current.digit1Key.wasPressedThisFrame) TrySelectUnitByIndex(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TrySelectUnitByIndex(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TrySelectUnitByIndex(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) TrySelectUnitByIndex(3);

        // Ability hotkeys — press to activate (or start charging)
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

        // Ability hotkeys — release to fire charge abilities
        if (Keyboard.current.qKey.wasReleasedThisFrame)
        {
            TryReleaseAbility(0);
        }
        if (Keyboard.current.wKey.wasReleasedThisFrame)
        {
            TryReleaseAbility(1);
        }
    }

    // Cached selectable units for number-key selection (refreshed each scene)
    private CharacterSelector[] selectableUnits;

    void TrySelectUnitByIndex(int index)
    {
        // Lazy-init or refresh if stale
        if (selectableUnits == null || selectableUnits.Length == 0)
            selectableUnits = FindObjectsByType<CharacterSelector>(FindObjectsSortMode.None);

        if (index < 0 || index >= selectableUnits.Length) return;
        if (selectableUnits[index] == null) return;

        SelectionManager.Instance.SelectCharacter(selectableUnits[index]);
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

    void TryReleaseAbility(int slotIndex)
    {
        if (SelectionManager.Instance.currentlySelected == null) return;

        Hero selectedHero = SelectionManager.Instance.currentlySelected.GetComponent<Hero>();
        if (selectedHero == null) return;

        selectedHero.ReleaseAbility(slotIndex);
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
        Hero selectedHero = SelectionManager.Instance.currentlySelected.gameObject.GetComponent<Hero>();

        // Check if we right-clicked on an enemy
        if (selectedHero.IsHealer == false && hit.collider != null && hit.collider.gameObject.GetComponent<Enemy>() != null)
        {
            Debug.Log("Right-clicked on an enemy.");
            //float range = 2f; // TEMP attack range ****************************COMMENTED OUT TO TEST STATE TRANSITION FROM CHASING TO ATTACKING
            Vector3 enemyPosition = hit.collider.transform.position;
            Vector3 unitPosition = SelectionManager.Instance.currentlySelected.transform.position;
            
            // Identify the currently selected Hero and the clicked enemy, set the enemy as the hero's target so that hero will aggro enemy
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
        else if(selectedHero.IsHealer == true && hit.collider != null && hit.collider.gameObject.GetComponent<Hero>() != null)
        {
            Hero clickedAlly = hit.collider.gameObject.GetComponent<Hero>();
            selectedHero.SetTarget(clickedAlly);
            SpawnIndicator(worldPosition, true);
        }
        // If not clicking on an enemy, move to the location as normal
        else
        {
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
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if we hit anything
        if (hit.collider == null)
        {
            SelectionManager.Instance.SelectCharacter(); // Deselect if we click on empty space
            return;
        }
        Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
        CharacterSelector clickedCharacter = hit.collider.GetComponent<CharacterSelector>();
        if (clickedCharacter != null)
        {
            if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.ctrlKey.isPressed)
            {
                Debug.Log("Pressing shift or control while clicking, adding to selection.");
                SelectionManager.Instance.AddToSelection(clickedCharacter);
            }
            else
            {
                Debug.Log("Left-clicked on a character: " + clickedCharacter.gameObject.name);
                SelectionManager.Instance.SelectCharacter(clickedCharacter);
            }
        }
        else
        {
            Debug.Log("Hit object has no CharacterSelector component.");
        }
    }

    private void SpawnIndicator(Vector3 position, bool isEnemy)
    {
        clickIndicator.Show(position, isEnemy);
    }
}
