using UnityEngine;

/// <summary>
/// Abstract base class for all enemies.
///
/// Handles: health, state machine, target acquisition, movement via IPathProvider,
/// and the attack loop. Subclasses override hooks to customize behavior
/// (melee swing, projectile fire, boss patterns, etc.)
///
/// State flow:  Idle -> Chasing -> Attacking -> (Chasing or Dead)
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    // ── State Machine ──────────────────────────────────────────────
    public enum EnemyState { Idle, Chasing, Attacking, Dead }
    public EnemyState CurrentState { get; protected set; } = EnemyState.Idle;

    // ── Stats ──────────────────────────────────────────────────────
    [Header("Enemy Stats (assign in inspector or set via code)")]
    [SerializeField] protected EnemyStats stats;

    protected float currentHealth;
    protected float attackTimer = 0f;

    // ── Targeting ──────────────────────────────────────────────────
    protected Transform currentTarget;

    // ── Pathfinding ────────────────────────────────────────────────
    protected IPathProvider pathProvider;

    // ══════════════════════════════════════════════════════════════
    //  LIFECYCLE
    // ══════════════════════════════════════════════════════════════

    protected virtual void Awake()
    {
        if (stats == null)
        {
            stats = CreateDefaultStats();
            Debug.Log($"[{gameObject.name}] No EnemyStats assigned — using defaults.");
        }

        currentHealth = stats.maxHealth;
        pathProvider = CreatePathProvider();

        Debug.Log($"[{stats.enemyName}] Spawned | HP: {currentHealth} | ATK: {stats.attackDamage} | SPD: {stats.moveSpeed}");
    }

    protected virtual void Update()
    {
        if (CurrentState == EnemyState.Dead) return;

        // Tick attack cooldown
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        switch (CurrentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Chasing:
                UpdateChasing();
                break;
            case EnemyState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  STATE UPDATES
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Idle: scan for a target within aggro range.
    /// </summary>
    protected virtual void UpdateIdle()
    {
        currentTarget = FindClosestUnit();

        if (currentTarget != null)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist <= stats.aggroRange)
            {
                TransitionTo(EnemyState.Chasing);
            }
        }
    }

    /// <summary>
    /// Chasing: move toward target using the path provider.
    /// Switch to Attacking when in range, or back to Idle if target is lost.
    /// </summary>
    protected virtual void UpdateChasing()
    {
        if (currentTarget == null)
        {
            TransitionTo(EnemyState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, currentTarget.position);

        // Lost aggro?
        if (dist > stats.aggroRange * 1.5f)
        {
            currentTarget = null;
            TransitionTo(EnemyState.Idle);
            return;
        }

        // In attack range?
        if (dist <= stats.attackRange)
        {
            TransitionTo(EnemyState.Attacking);
            return;
        }

        // Move toward target
        Vector2 moveDir = pathProvider.GetMoveDirection(
            transform.position,
            currentTarget.position,
            stats.attackRange * 0.9f // stop slightly inside attack range
        );

        transform.position += (Vector3)(moveDir * stats.moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Attacking: auto-attack on cooldown while in range.
    /// Return to Chasing if the target moves out of range.
    /// </summary>
    protected virtual void UpdateAttacking()
    {
        if (currentTarget == null)
        {
            TransitionTo(EnemyState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, currentTarget.position);

        if (dist > stats.attackRange * 1.2f)
        {
            // Target moved away — chase again
            TransitionTo(EnemyState.Chasing);
            return;
        }

        // Attack if cooldown is ready
        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = stats.attackCooldown;
        }
    }

    // ══════════════════════════════════════════════════════════════
    //  HOOKS FOR SUBCLASSES
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Override to define the path provider for this enemy type.
    /// Melee enemies return DirectPursuitPath, ranged return KeepDistancePath, etc.
    /// </summary>
    protected abstract IPathProvider CreatePathProvider();

    /// <summary>
    /// Override to define what happens when the enemy attacks.
    /// Melee: deal damage to target. Ranged: spawn a projectile. Boss: do a pattern.
    /// </summary>
    protected abstract void PerformAttack();

    /// <summary>
    /// Override to return default stats if none are assigned in the inspector.
    /// </summary>
    protected abstract EnemyStats CreateDefaultStats();

    /// <summary>
    /// Called when the enemy dies. Override to add death animations, loot drops, etc.
    /// Base implementation logs and disables the GameObject.
    /// </summary>
    protected virtual void OnDeath()
    {
        Debug.Log($"[{stats.enemyName}] Died!");
        // Disable rather than destroy — lets you pool enemies later
        gameObject.SetActive(false);
    }

    // ══════════════════════════════════════════════════════════════
    //  PUBLIC API
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Call this to deal damage to the enemy.
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (CurrentState == EnemyState.Dead) return;

        currentHealth -= damage;
        Debug.Log($"[{stats.enemyName}] Took {damage} damage! HP: {currentHealth}/{stats.maxHealth}");

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            TransitionTo(EnemyState.Dead);
            OnDeath();
        }
    }

    public float GetHealthPercent()
    {
        return stats != null ? currentHealth / stats.maxHealth : 0f;
    }

    public bool IsAlive => CurrentState != EnemyState.Dead;

    // ══════════════════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════════════════

    protected void TransitionTo(EnemyState newState)
    {
        if (CurrentState == newState) return;
        Debug.Log($"[{stats.enemyName}] {CurrentState} -> {newState}");
        CurrentState = newState;
    }

    /// <summary>
    /// Finds the closest player-controlled UnitController in the scene.
    /// Replace this with a proper team/target manager later.
    /// </summary>
    protected Transform FindClosestUnit()
    {
        UnitController[] units = FindObjectsByType<UnitController>(FindObjectsSortMode.None);
        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var unit in units)
        {
            float dist = Vector2.Distance(transform.position, unit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = unit.transform;
            }
        }

        return closest;
    }
}
