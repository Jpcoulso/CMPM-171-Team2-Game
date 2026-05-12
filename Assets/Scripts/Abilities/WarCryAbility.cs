using UnityEngine;

// WarCryAbility.cs — LEGACY: Old Warrior E implementation.
// Replaced by WarCryData.cs (AbilityData ScriptableObject).
// Kept for reference. Safe to delete once fully migrated.

public class WarCryAbility : Ability
{
    private const float Duration = 0.5f;

    public WarCryAbility(string name, float cooldown, string key)
        : base(name, cooldown, key) { }

    protected override void Execute()
    {
        if (owner == null) return;

        AbilityVFX.SpawnRing(owner.transform.position, 0.5f, 3.5f,
            new Color(1f, 0.9f, 0.2f, 0.7f), Duration);

        AbilityVFX.SpawnSquare(owner.transform.position, 1.0f,
            new Color(1f, 1f, 0.5f, 0.5f), 0.2f);
    }
}
