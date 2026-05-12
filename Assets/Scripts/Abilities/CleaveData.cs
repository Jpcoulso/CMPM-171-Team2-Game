using UnityEngine;

// CleaveData.cs — Warrior W ability.
// Wide slash aimed toward the mouse cursor. Bigger area than Shield Bash.
// In Unity: Right Click -> Create -> RPG/Abilities/Cleave

[CreateAssetMenu(fileName = "Cleave", menuName = "RPG/Abilities/Cleave")]
public class CleaveData : AbilityData
{
    [Header("Cleave Settings")]
    public float damage = 35f;
    public float width = 2.5f;
    public float height = 1.0f;
    public float range = 1.2f;
    public float vfxDuration = 0.35f;

    public override void Execute(Character owner)
    {
        Vector2 facing = AbilityUtils.GetAimDirection(owner.gameObject);
        Vector3 spawnPos = owner.transform.position + (Vector3)(facing * range);

        // Orange slash VFX, rotated to face the mouse
        var vfx = AbilityVFX.SpawnSquare(spawnPos, 1f, new Color(1f, 0.6f, 0.1f, 0.85f), vfxDuration);
        vfx.transform.localScale = new Vector3(width, height, 1f);
        float angle = Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;
        vfx.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Damage anything in the hitbox area
        var hitbox = new GameObject("Cleave_Hit");
        hitbox.transform.position = spawnPos;
        hitbox.transform.localScale = new Vector3(width, height, 1f);
        hitbox.AddComponent<AbilityHitbox>().Initialize(damage, 0.05f, owner.gameObject);
    }

    public override void ApplyPassive(Character owner) { }
}
