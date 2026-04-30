using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using NUnit.Framework.Internal;

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actions;
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
        var map = actions.FindActionMap("Player");
        LeftClick = map.FindAction("LeftClick");
        RightClick = map.FindAction("RightClick");
        Stop = map.FindAction("Stop");

    }
    void OnEnable()
    {
        RightClick.Enable();
        LeftClick.Enable();
        Stop.Enable();
    }
    void OnDisable()
    {
        LeftClick.Disable();
        RightClick.Disable();
        Stop.Disable();
    }
    void Update()
    {
        if (LeftClick.WasPressedThisFrame())
        {
            OnLeftClick();
        }
        if (RightClick.WasPressedThisFrame())
        {
            OnRightClick();
        }
        if (Stop.WasPressedThisFrame())
        {
            OnStop();
        }

        // --- Ability key inputs ---
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.qKey.wasPressedThisFrame) UseAbilityOnActiveUnit(0);
        if (kb.wKey.wasPressedThisFrame) UseAbilityOnActiveUnit(1);
        if (kb.eKey.wasPressedThisFrame) UseAbilityOnActiveUnit(2);
        if (kb.rKey.wasPressedThisFrame) UseAbilityOnActiveUnit(3);
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

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Debug.Log("Right-clicked on an enemy.");
            float range = 2f; // TEMP attack range
            Vector3 enemyPosition = hit.collider.transform.position;
            Vector3 unitPosition = SelectionManager.Instance.currentlySelected.transform.position;

            Vector3 directionToEnemy = (enemyPosition - unitPosition).normalized;
            Vector3 destination = enemyPosition - directionToEnemy * range;

            SpawnIndicator(worldPosition, true); // Spawn a red indicator for enemies
            // Move the selected unit into attack range
            if (SelectionManager.Instance == null || SelectionManager.Instance.currentlySelected == null)
        {
            Debug.Log("No unit selected — left-click a unit first.");
            return;
        }
        UnitController unitController = SelectionManager.Instance.currentlySelected.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.Move(destination);
            }   
        }
        // If not clicking on an enemy, move to the location as normal
        else
        {
            UnitController unitController = SelectionManager.Instance.currentlySelected.GetComponent<UnitController>();
            if (unitController != null)
            {
                SpawnIndicator(worldPosition, false); // Spawn a green indicator for allies
                unitController.Move(worldPosition);
            }
        }
    }
    void OnLeftClick()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Debug.Log("Left Click at: " + mousePosition);
        // Implement left-click logic here
    }
    void OnStop()
    {
        UnitController unitController = UnityEngine.Object.FindFirstObjectByType<UnitController>();
        if (unitController != null)
        {
            unitController.Stop();
        }
    }
    private void SpawnIndicator(Vector3 position, bool isEnemy)
    {
        ClickIndicator indicator = Instantiate(clickIndicatorPrefab, position, Quaternion.identity);
        indicator.SetColor(isEnemy); // Set color based on whether it's an enemy or not
    }

    // Finds the active unit and fires the ability at the given slot.
    // Slot: 0=Q, 1=W, 2=E, 3=R
    private void UseAbilityOnActiveUnit(int slot)
    {
        // Find the current active unit's AbilityHolder
        AbilityHolder holder = UnityEngine.Object.FindFirstObjectByType<AbilityHolder>();
        if (holder != null)
        {
            holder.UseAbility(slot);
        }
        else
        {
            Debug.LogWarning("No AbilityHolder found on any unit!");
        }
    }
}
