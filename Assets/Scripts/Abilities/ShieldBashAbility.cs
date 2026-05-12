using UnityEngine;

// ShieldBashAbility.cs — LEGACY: Old Warrior Q implementation.
// Replaced by ShieldBashData.cs (AbilityData ScriptableObject).
// Kept for reference. Safe to delete once fully migrated.

public class ShieldBashAbility : Ability
{
    private const float Damage = 25f;
    private const float HitboxSize = 1.2f;
    private const float Range = 1.5f;
    private const float Duration = 0.3f;

    public ShieldBashAbility(string name, float cooldown, string key)
        : base(name, cooldown, key) { }

    protected override void Execute()
    {
        if (owner == null) return;

        Vector2 facing = AbilityUtils.GetAimDirection(owner);
        Vector3 spawnPos = owner.transform.position + (Vector3)(facing * Range);

        AbilityVFX.SpawnSquare(spawnPos, HitboxSize, new Color(1f, 0.2f, 0.2f, 0.8f), Duration);

        var hitbox = new GameObject("ShieldBash_Hit");
        hitbox.transform.position = spawnPos;
        hitbox.transform.localScale = new Vector3(HitboxSize, HitboxSize, 1f);
        hitbox.AddComponent<AbilityHitbox>().Initialize(Damage, 0.05f, owner);
    }
}
