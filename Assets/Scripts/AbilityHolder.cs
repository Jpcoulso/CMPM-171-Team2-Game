using UnityEngine;

// AbilityHolder.cs — LEGACY: Old ability container.
// Replaced by Hero.cs + AbilityHandler system.
// Kept for reference. Safe to delete once fully migrated.

public class AbilityHolder : MonoBehaviour
{
    public Ability abilityQ;
    public Ability abilityW;
    public Ability abilityE;
    public Ability abilityR;

    public void Initialize(UnitClassData unitClass)
    {
        abilityQ = AbilityFactory.Create(unitClass.className, 0, unitClass.qName, unitClass.qCooldown, "Q");
        abilityW = AbilityFactory.Create(unitClass.className, 1, unitClass.wName, unitClass.wCooldown, "W");
        abilityE = AbilityFactory.Create(unitClass.className, 2, unitClass.eName, unitClass.eCooldown, "E");
        abilityR = AbilityFactory.Create(unitClass.className, 3, unitClass.rName, unitClass.rCooldown, "R");

        abilityQ.owner = gameObject;
        abilityW.owner = gameObject;
        abilityE.owner = gameObject;
        abilityR.owner = gameObject;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        abilityQ?.UpdateCooldown(dt);
        abilityW?.UpdateCooldown(dt);
        abilityE?.UpdateCooldown(dt);
        abilityR?.UpdateCooldown(dt);
    }

    public void UseAbility(int slot)
    {
        switch (slot)
        {
            case 0: abilityQ?.TryUse(); break;
            case 1: abilityW?.TryUse(); break;
            case 2: abilityE?.TryUse(); break;
            case 3: abilityR?.TryUse(); break;
        }
    }

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
