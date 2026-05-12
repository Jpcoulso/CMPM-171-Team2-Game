using UnityEngine;

// BerserkerRageData.cs — Warrior R ability (ultimate).
// AOE burst damage around the caster + temporary 1.5x damage buff.
// Red border shows while the buff is active.
// In Unity: Right Click -> Create -> RPG/Abilities/Berserker Rage

[CreateAssetMenu(fileName = "BerserkerRage", menuName = "RPG/Abilities/Berserker Rage")]
public class BerserkerRageData : AbilityData
{
    [Header("Berserker Rage Settings")]
    public float damage = 60f;
    public float hitboxRadius = 3f;
    public float vfxDuration = 0.6f;
    public float damageMultiplier = 1.5f;
    public float buffDuration = 2f;

    public override void Execute(Character owner)
    {
        Vector3 center = owner.transform.position;

        // Apply damage multiplier buff (red border appears on the character)
        owner.ApplyDamageMultiplier(damageMultiplier, buffDuration);

        // Big red expanding ring
        AbilityVFX.SpawnRing(center, 0.5f, 5f,
            new Color(1f, 0.1f, 0.1f, 0.9f), vfxDuration);

        // Secondary orange ring
        AbilityVFX.SpawnRing(center, 1f, 3.5f,
            new Color(1f, 0.4f, 0.1f, 0.6f), vfxDuration * 0.7f);

        // Flash on the unit
        AbilityVFX.SpawnSquare(center, 1.5f,
            new Color(1f, 0.2f, 0.1f, 0.7f), 0.25f);

        // AOE damage to all enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitboxRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner.gameObject) continue;

            Character target = hit.GetComponent<Character>();
            if (target != null && !target.IsDead)
                target.TakeDamage(damage);
        }
    }

    public override void ApplyPassive(Character owner) { }
}
