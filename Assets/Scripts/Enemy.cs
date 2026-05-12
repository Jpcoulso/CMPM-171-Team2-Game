using UnityEngine;
using System.Collections.Generic;
// Enemy.cs
// Goes on your Goblin GameObject.
// Drag Goblin.asset into the enemyData field.

public class Enemy : Character
{
    [SerializeField] private EnemyData enemyData;

    public override float MaxHealth    => enemyData.maxHealth;
    public override float AttackDamage => enemyData.attackDamage;
    public override float MoveSpeed    => enemyData.moveSpeed;
    public override float AttackRange => enemyData.attackRange;
    public override float AttackRate => enemyData.attackRate;

    public override string GetCharacterName() => enemyData.enemyName;

// ─────────────────────────────────────────
    // AI SCANNING
    // ─────────────────────────────────────────

    private float scanInterval = 0.5f;  // scan every half second, not every frame
    private float scanTimer;            // performance friendly


    private void Start()
    {
        currentHealth = MaxHealth;
        scanTimer = scanInterval;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();          // runs the state machine from Character.cs

        // Only scan for targets when Idle — no point scanning mid-combat
        if (CurrentState == CharacterState.Idle)
            TickScan();
    }

    private void TickScan()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer > 0) return;

        scanTimer = scanInterval;
        ScanForHeroes();
    }

    private void ScanForHeroes()
    {
        IReadOnlyList<Hero> squad = SquadManager.Instance.GetSquad();
        Hero closest = null;
        float closestDistance = enemyData.detectionRange;

        foreach (Hero hero in squad)
        {
            if (hero == null || hero.IsDead) continue;

            float distance = Vector2.Distance(transform.position,
                                              hero.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hero;
            }
        }

        // Found a hero within detection range — begin chasing
        if (closest != null)
        {
            SetTarget(closest);
            Debug.Log("Enemy found target");
        }
            
    }

    protected override void OnDeath()
    {
        
        Debug.Log($"{GetCharacterName()} defeated! " +
                  $"Rewarding {enemyData.experienceReward} XP.");
        Destroy(gameObject); //called via animation event
        WaveManager.Instance.OnEnemyDied(this);
        //gameObject.SetActive(false);
    }
}
