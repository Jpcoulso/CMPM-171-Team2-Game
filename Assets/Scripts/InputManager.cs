using UnityEngine;
using UnityEngine.InputSystem;

// InputManager.cs — Handles all mouse and keyboard input.
// Left click: select a unit. Right click: move or attack. S: stop.
// Q/W/E/R: abilities on unit 1. SHIFT+: unit 2. CTRL+: unit 3.

public class InputManager : MonoBehaviour
{
    [SerializeField] private ClickIndicator clickIndicatorPrefab;

    public static InputManager Instance { get; private set; }

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
            OnLeftClick();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            OnRightClick();

        if (Keyboard.current.sKey.wasPressedThisFrame)
            OnStop();

        HandleAbilityKeys();
    }

    // =============================================
    //  LEFT CLICK — Select a unit
    // =============================================

    void OnLeftClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null)
        {
            SelectionManager.Instance.SelectCharacter(); // Deselect
            return;
        }

        CharacterSelector clickedCharacter = hit.collider.GetComponent<CharacterSelector>();
        if (clickedCharacter != null)
            SelectionManager.Instance.SelectCharacter(clickedCharacter);
    }

    // =============================================
    //  RIGHT CLICK — Move or attack
    // =============================================

    void OnRightClick()
    {
        if (SelectionManager.Instance.currentlySelected == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        worldPos.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        Hero selectedHero = SelectionManager.Instance.currentlySelected.GetComponent<Hero>();
        if (selectedHero == null) return;

        // Right-clicked on an enemy — attack it
        if (hit.collider != null && hit.collider.GetComponent<Enemy>() != null)
        {
            Enemy clickedEnemy = hit.collider.GetComponent<Enemy>();
            selectedHero.SetTarget(clickedEnemy);
            SpawnIndicator(worldPos, true);
        }
        // Right-clicked on ground — move there
        else
        {
            selectedHero.SetDestination(worldPos);
            SpawnIndicator(worldPos, false);
        }
    }

    // =============================================
    //  STOP — S key
    // =============================================

    void OnStop()
    {
        if (SelectionManager.Instance.currentlySelected == null)
            return;

        Hero selectedHero = SelectionManager.Instance.currentlySelected.GetComponent<Hero>();
        if (selectedHero != null)
            selectedHero.ClearTarget();
    }

    // =============================================
    //  ABILITIES — Q/W/E/R with modifier keys
    // =============================================

    // No modifier = unit 0, SHIFT = unit 1, CTRL = unit 2
    // Q = slot 0, W = slot 1, E = slot 2, R = slot 3
    void HandleAbilityKeys()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        bool shift = kb.shiftKey.isPressed;
        bool ctrl = kb.ctrlKey.isPressed;

        if (kb.qKey.wasPressedThisFrame)
        {
            if (ctrl)       UseAbilityOnUnit(2, 0);
            else if (shift) UseAbilityOnUnit(1, 0);
            else            UseAbilityOnUnit(0, 0);
        }
        if (kb.wKey.wasPressedThisFrame)
        {
            if (ctrl)       UseAbilityOnUnit(2, 1);
            else if (shift) UseAbilityOnUnit(1, 1);
            else            UseAbilityOnUnit(0, 1);
        }
        if (kb.eKey.wasPressedThisFrame)
        {
            if (ctrl)       UseAbilityOnUnit(2, 2);
            else if (shift) UseAbilityOnUnit(1, 2);
            else            UseAbilityOnUnit(0, 2);
        }
        if (kb.rKey.wasPressedThisFrame)
        {
            if (ctrl)       UseAbilityOnUnit(2, 3);
            else if (shift) UseAbilityOnUnit(1, 3);
            else            UseAbilityOnUnit(0, 3);
        }
    }

    // Tells a hero in the squad to use an ability by slot index
    private void UseAbilityOnUnit(int unitIndex, int slot)
    {
        if (SquadManager.Instance == null) return;

        var squad = SquadManager.Instance.GetSquad();
        if (unitIndex < 0 || unitIndex >= squad.Count || squad[unitIndex] == null)
            return;

        squad[unitIndex].UseAbility(slot);
    }

    // =============================================
    //  CLICK INDICATOR
    // =============================================

    private void SpawnIndicator(Vector3 position, bool isEnemy)
    {
        ClickIndicator indicator = Instantiate(clickIndicatorPrefab, position, Quaternion.identity);
        indicator.SetColor(isEnemy);
    }
}
