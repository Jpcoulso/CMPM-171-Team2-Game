using UnityEngine;

// Basic melee enemy: chases the closest player unit and auto-attacks
// when in range. Uses DirectPursuitPath for movement (straight line
// toward target).
//
// To create a new melee variant, either:
//   1. Create a new EnemyStats SO with different values, or
//   2. Subclass this and override PerformAttack for unique melee behavior.
public class MeleeEnemy : EnemyBase
{
    protected override IPathProvider CreatePathProvider()
    {
        return new DirectPursuitPath();
    }

    protected override void PerformAttack()
    {
        if (currentTarget == null) return;

        Debug.Log($"[{stats.enemyName}] Melee attack on {currentTarget.name} for {stats.attackDamage} damage!");

        // TODO: Replace with actual damage dealing when the player health system exists.
        // Example:
        //   var health = currentTarget.GetComponent<HealthComponent>();
        //   if (health != null) health.TakeDamage(stats.attackDamage);
    }

    protected override EnemyStats CreateDefaultStats()
    {
        var defaultStats = ScriptableObject.CreateInstance<EnemyStats>();
        defaultStats.enemyName = "Melee Grunt";
        defaultStats.maxHealth = 50f;
        defaultStats.moveSpeed = 3f;
        defaultStats.attackDamage = 10f;
        defaultStats.attackRange = 1.5f;
        defaultStats.attackCooldown = 1.5f;
        defaultStats.aggroRange = 12f;
        defaultStats.expReward = 10;
        return defaultStats;
    }

    protected override void OnDeath()
    {
        Debug.Log($"[{stats.enemyName}] Melee enemy slain! (+{stats.expReward} exp)");
        base.OnDeath();
    }
}
