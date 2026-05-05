using UnityEngine;

// Static helper that creates default UnitClassData instances at runtime
// for testing purposes. No need to create ScriptableObject assets manually.
// When you're ready to move past the skeleton, create actual SO assets
// in the editor instead (Assets > Create > Unit Class Data).
public static class UnitClassDefaults
{
    public static UnitClassData CreateWarrior()
    {
        var data = ScriptableObject.CreateInstance<UnitClassData>();
        data.className = "Warrior";
        data.classIcon = LoadIcon("Warrior_Icon");
        data.qName = "Shield Bash";
        data.qCooldown = 3f;
        data.qIcon = LoadIcon("Warrior_Q");
        data.wName = "Cleave";
        data.wCooldown = 6f;
        data.wIcon = LoadIcon("Warrior_W");
        data.eAbilityName = "War Cry";
        data.eCooldown = 8f;
        data.eIcon = LoadIcon("Warrior_E");
        data.rName = "Berserker Rage";
        data.rCooldown = 120f;
        data.rIcon = LoadIcon("Warrior_R");
        return data;
    }

    private static Sprite LoadIcon(string name)
    {
        // Loads from Assets/Resources/Icons/<name>.png
        // Unity strips the extension — just pass the path relative to Resources/
        Sprite sprite = Resources.Load<Sprite>($"Icons/{name}");
        if (sprite == null)
            Debug.LogWarning($"Could not load icon: Icons/{name}");
        return sprite;
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
