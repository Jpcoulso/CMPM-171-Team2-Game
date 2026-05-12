using UnityEngine;

// ShieldBashData.cs — Warrior Q ability.
// Short-range hit aimed toward the mouse cursor.
// In Unity: Right Click -> Create -> RPG/Abilities/Shield Bash

[CreateAssetMenu(fileName = "ShieldBash", menuName = "RPG/Abilities/Shield Bash")]
public class ShieldBashData : AbilityData
{
    [Header("Shield Bash Settings")]
    public float damage = 25f;
    public float hitboxSize = 1.2f;
    public float range = 1.5f;
    public float vfxDuration = 0.3f;

    public override void Execute(Character owner)
    {
        Vector2 facing = AbilityUtils.GetAimDirection(owner.gameObject);
        Vector3 spawnPos = owner.transform.position + (Vector3)(facing * range);

        // Red flash VFX
        AbilityVFX.SpawnSquare(spawnPos, hitboxSize, new Color(1f, 0.2f, 0.2f, 0.8f), vfxDuration);

        // Damage anything in the hitbox area
        var hitbox = new GameObject("ShieldBash_Hit");
        hitbox.transform.position = spawnPos;
        hitbox.transform.localScale = new Vector3(hitboxSize, hitboxSize, 1f);
        hitbox.AddComponent<AbilityHitbox>().Initialize(damage, 0.05f, owner.gameObject);
    }

    public override void ApplyPassive(Character owner) { }
}
