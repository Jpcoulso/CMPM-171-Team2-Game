using UnityEngine;

// BerserkerRageAbility.cs — LEGACY: Old Warrior R implementation.
// Replaced by BerserkerRageData.cs (AbilityData ScriptableObject).
// Kept for reference. Safe to delete once fully migrated.

public class BerserkerRageAbility : Ability
{
    private const float Damage = 60f;
    private const float HitboxRadius = 3f;
    private const float Duration = 0.6f;

    public BerserkerRageAbility(string name, float cooldown, string key)
        : base(name, cooldown, key) { }

    protected override void Execute()
    {
        if (owner == null) return;

        Vector3 center = owner.transform.position;

        AbilityVFX.SpawnRing(center, 0.5f, 5f,
            new Color(1f, 0.1f, 0.1f, 0.9f), Duration);

        AbilityVFX.SpawnRing(center, 1f, 3.5f,
            new Color(1f, 0.4f, 0.1f, 0.6f), Duration * 0.7f);

        AbilityVFX.SpawnSquare(center, 1.5f,
            new Color(1f, 0.2f, 0.1f, 0.7f), 0.25f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, HitboxRadius);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;

            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null && enemy.IsAlive)
                enemy.TakeDamage(Damage);
        }
    }
}
