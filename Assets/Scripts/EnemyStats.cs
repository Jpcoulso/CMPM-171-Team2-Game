using UnityEngine;

/// <summary>
/// ScriptableObject that defines an enemy type's stats.
/// Create via Assets > Create > Enemy Stats in the Unity editor.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Identity")]
    public string enemyName = "Enemy";

    [Header("Health")]
    public float maxHealth = 50f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackRange = 1.5f;    // how close to be to attack
    public float attackCooldown = 1.5f; // seconds between attacks
    public float aggroRange = 15f;      // how far away the enemy detects a player unit

    [Header("Loot / Rewards (placeholder)")]
    public int expReward = 10;
}
