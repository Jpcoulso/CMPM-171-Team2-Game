using UnityEngine;

// Static helper that creates default UnitClassData instances at runtime
// for testing purposes. No need to create ScriptableObject assets manually.
//
// When you're ready to move past the skeleton, create actual SO assets
// in the editor instead (Assets > Create > Unit Class Data).
public static class UnitClassDefaults
{
    public static UnitClassData CreateWarrior()
    {
        var data = ScriptableObject.CreateInstance<UnitClassData>();
        data.className = "Warrior";
        data.qName = "Shield Bash";
        data.qCooldown = 3f;
        data.wName = "Cleave";
        data.wCooldown = 6f;
        data.eAbilityName = "War Cry";
        data.eCooldown = 8f;
        data.rName = "Berserker Rage";
        data.rCooldown = 120f;
        return data;
    }

    public static UnitClassData CreateMage()
    {
        var data = ScriptableObject.CreateInstance<UnitClassData>();
        data.className = "Mage";
        data.qName = "Fireball";
        data.qCooldown = 2f;
        data.wName = "Frost Nova";
        data.wCooldown = 7f;
        data.eAbilityName = "Blink";
        data.eCooldown = 10f;
        data.rName = "Meteor Strike";
        data.rCooldown = 240f;
        return data;
    }

    public static UnitClassData CreateRanger()
    {
        var data = ScriptableObject.CreateInstance<UnitClassData>();
        data.className = "Ranger";
        data.qName = "Piercing Shot";
        data.qCooldown = 4f;
        data.wName = "Trap";
        data.wCooldown = 5f;
        data.eAbilityName = "Dash";
        data.eCooldown = 9f;
        data.rName = "Rain of Arrows";
        data.rCooldown = 60f;
        return data;
    }
}
