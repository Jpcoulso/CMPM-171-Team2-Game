using UnityEngine;

// CleaveAbility.cs — LEGACY: Old Warrior W implementation.
// Replaced by CleaveData.cs (AbilityData ScriptableObject).
// Kept for reference. Safe to delete once fully migrated.

public class CleaveAbility : Ability
{
    private const float Damage = 35f;
    private const float Width = 2.5f;
    private const float Height = 1.0f;
    private const float Range = 1.2f;
    private const float Duration = 0.35f;

    public CleaveAbility(string name, float cooldown, string key)
        : base(name, cooldown, key) { }

    protected override void Execute()
    {
        if (owner == null) return;

        Vector2 facing = AbilityUtils.GetAimDirection(owner);
        Vector3 spawnPos = owner.transform.position + (Vector3)(facing * Range);

        var vfx = AbilityVFX.SpawnSquare(spawnPos, 1f, new Color(1f, 0.6f, 0.1f, 0.85f), Duration);
        vfx.transform.localScale = new Vector3(Width, Height, 1f);
        float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        vfx.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        var hitbox = new GameObject("Cleave_Hit");
        hitbox.transform.position = spawnPos;
        hitbox.transform.localScale = new Vector3(Width, Height, 1f);
        hitbox.AddComponent<AbilityHitbox>().Initialize(Damage, 0.05f, owner);
    }
}
