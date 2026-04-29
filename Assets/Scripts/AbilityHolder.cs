using UnityEngine;

// Attach this to a unit to give it Q/W/E/R abilities.
// Handles cooldown ticking and ability activation.
public class AbilityHolder : MonoBehaviour
{
    public Ability abilityQ;
    public Ability abilityW;
    public Ability abilityE;
    public Ability abilityR;

    // Initialize this holder with a set of 4 abilities from a UnitClassData.
    public void Initialize(UnitClassData unitClass)
    {
        abilityQ = new Ability(unitClass.qName, unitClass.qCooldown, "Q");
        abilityW = new Ability(unitClass.wName, unitClass.wCooldown, "W");
        abilityE = new Ability(unitClass.eName, unitClass.eCooldown, "E");
        abilityR = new Ability(unitClass.rName, unitClass.rCooldown, "R");

        Debug.Log($"== {unitClass.className} Abilities Loaded ==");
        Debug.Log($"  Q: {abilityQ.abilityName} ({abilityQ.cooldownDuration}s cd)");
        Debug.Log($"  W: {abilityW.abilityName} ({abilityW.cooldownDuration}s cd)");
        Debug.Log($"  E: {abilityE.abilityName} ({abilityE.cooldownDuration}s cd)");
        Debug.Log($"  R: {abilityR.abilityName} ({abilityR.cooldownDuration}s cd)");
    }

    void Update()
    {
        // Tick all cooldowns
        float dt = Time.deltaTime;
        abilityQ?.UpdateCooldown(dt);
        abilityW?.UpdateCooldown(dt);
        abilityE?.UpdateCooldown(dt);
        abilityR?.UpdateCooldown(dt);
    }

    // Called by InputManager when the player presses an ability key.
    public void UseAbility(int slot)
    {
        switch (slot)
        {
            case 0: abilityQ?.TryUse(); break;
            case 1: abilityW?.TryUse(); break;
            case 2: abilityE?.TryUse(); break;
            case 3: abilityR?.TryUse(); break;
            default:
                Debug.LogWarning($"Invalid ability slot: {slot}");
                break;
        }
    }

    // Get ability by slot index (0=Q, 1=W, 2=E, 3=R).
    // Useful for UI or status checks.
    public Ability GetAbility(int slot)
    {
        return slot switch
        {
            0 => abilityQ,
            1 => abilityW,
            2 => abilityE,
            3 => abilityR,
            _ => null
        };
    }
}
