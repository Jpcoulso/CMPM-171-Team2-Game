using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class BossAttack
{
    public string name;
    public string animTrigger;
    public float damage;
    public float range;
    public float cooldown;
    [HideInInspector] public float currentCooldown;
}
 
public class DemonBoss : Enemy
{
    [Header("Boss Attacks")]
    [SerializeField] private BossAttack cleaveAttack;
    [SerializeField] private BossAttack fireballAttack;
    [SerializeField] private BossAttack jumpAttack;
    [SerializeField] private BossAttack breathAttack;
 
    [Header("Jump AoE")]
    [SerializeField] private float jumpRadius = 3f;
    [SerializeField] private GameObject jumpVFXPrefab;
 
    [Header("Firebreath")]
    [SerializeField] private Vector2 breathSize = new Vector2(3f, 1.5f);
    [SerializeField] private GameObject breathVFXPrefab;
 
    // Convenience list for cooldown ticking
    private List<BossAttack> allAttacks;
 
    // The attack currently being executed — AnimationEventRelay uses this
    private BossAttack activeAttack;
 
    public override float AttackRange => GetDynamicRange();
    private float introTimer = 3.0f;

    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------
    protected void Awake()
    {
        allAttacks = new List<BossAttack> { cleaveAttack, fireballAttack, jumpAttack, breathAttack };
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    protected override void Start()
    {
        base.Start();
    }
 
    protected override void FixedUpdate()
    {
        // If the intro is still playing, count down and do nothing else
        if (introTimer > 0)
        {
            introTimer -= Time.deltaTime;
            return; // This blocks the AI and state machine from starting
        }

        //once timer reaches zero start normal boss behavior
        base.FixedUpdate();
        TickAttackCooldowns();
    }
 
    // -------------------------------------------------------------------------
    // Cooldown Management
    // -------------------------------------------------------------------------
 
    private void TickAttackCooldowns()
    {
        foreach (BossAttack attack in allAttacks)
        {
            if (attack.currentCooldown > 0f)
                attack.currentCooldown -= Time.deltaTime;
        }
    }
 
    private bool IsReady(BossAttack attack) => attack.currentCooldown <= 0f;
 
    // -------------------------------------------------------------------------
    // Attack Selection — overrides the base Enemy's single-attack logic
    // -------------------------------------------------------------------------
 
    protected override void PerformAttack()
    {
        if (currentTarget == null) return;
 
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        BossAttack selectedAttack = SelectAttack(distance);
 
        if (selectedAttack == null) return;
 
        activeAttack = selectedAttack;
        selectedAttack.currentCooldown = selectedAttack.cooldown;
        attackCooldown = selectedAttack.cooldown; // keep base class cooldown in sync
 
        animator.SetTrigger(selectedAttack.animTrigger);
    }
 
    private BossAttack SelectAttack(float distance)
    {
        // Filter for attacks that are off cooldown AND in range
        List<BossAttack> validAttacks = new List<BossAttack>();
        foreach (BossAttack attack in allAttacks)
        {
            if (IsReady(attack) && distance <= attack.range)
            {
                validAttacks.Add(attack);
            }
        }

        // Pick one randomly from the valid options
        if (validAttacks.Count > 0)
        {
            return validAttacks[Random.Range(0, validAttacks.Count)];
        }
 
        return null;
    }

    private float GetDynamicRange()
    {
        float max = 0;
        foreach (var a in allAttacks)
        {
            if (IsReady(a)) max = Mathf.Max(max, a.range);
        }
        return max > 0 ? max : (cleaveAttack != null ? cleaveAttack.range : 2f);
    }
 
    // -------------------------------------------------------------------------
    // Attack Impacts — called by AnimationEventRelay
    // -------------------------------------------------------------------------
 
    // Single-target melee hit on the current target
    public void OnCleaveImpact()
    {
        if (currentTarget == null) return;
 
        currentTarget.TakeDamage(cleaveAttack.damage, this);
    }
 
    // Spawns a projectile that flies toward EVERY living hero
    public void OnFireballLaunch()
    {
        if (ProjectilePrefab == null) return;
 
        IReadOnlyList<Hero> squad = SquadManager.Instance.GetSquad();
        foreach (Hero hero in squad)
        {
            if (hero == null || hero.IsDead) continue;

            GameObject projectileObj = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
 
            if (projectile != null)
                projectile.Initialize(hero, fireballAttack.damage, this);
        }
    }
 
    // Circular AoE centred on the boss — hits all Heroes within jumpRadius
    public void OnJumpImpact()
    {
        if (jumpVFXPrefab != null)
            Instantiate(jumpVFXPrefab, transform.position, Quaternion.identity);
 
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, jumpRadius);
 
        foreach (Collider2D hit in hits)
        {
            Hero hero = hit.GetComponent<Hero>();
            if (hero != null)
                hero.TakeDamage(jumpAttack.damage, this);
        }
    }
 
    // Rectangular AoE in front of the boss — always faces the correct direction
    public void OnBreathImpact()
    {
        // Flip the offset so breath comes from the front regardless of facing direction
        float facingDirection = transform.localScale.x > 0 ? -1f : 1f;
        Vector3 offset = new Vector3((breathSize.x / 2f) * facingDirection, 0f, 0f);
        Vector2 boxCenter = transform.position + offset;
 
        if (breathVFXPrefab != null)
            Instantiate(breathVFXPrefab, boxCenter, Quaternion.identity);
 
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, breathSize, 0f);
 
        foreach (Collider2D hit in hits)
        {
            Hero hero = hit.GetComponent<Hero>();
            if (hero != null)
                hero.TakeDamage(breathAttack.damage, this);
        }
    }
 
    // overridden face target function to properly manage which way boss is facing (the sprite is default looking the opposite way as our enemy sprites)
    public override void FaceTarget(Vector3 position)
    {
        if (position.x < transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face left (default)
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face right (flipped)
    }

    protected override Vector3 CalcEngagementPoint(Vector3 targetPosition)
    {
        float range = (activeAttack != null) ? activeAttack.range : AttackRange;
        float targetOffset = range * 0.5f;
        if(transform.position.x < targetPosition.x)
        {
            return new Vector3(targetPosition.x - targetOffset, targetPosition.y, 0);
        }
        return new Vector3(targetPosition.x + targetOffset, targetPosition.y, 0);
    }

    // -------------------------------------------------------------------------
    // Debug Visualization
    // -------------------------------------------------------------------------
 
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Jump AoE radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, jumpRadius);
 
        // Firebreath box
        float facing = transform.localScale.x > 0 ? -1f : 1f;
        Vector3 breathOffset = new Vector3((breathSize.x / 2f) * facing, 0f, 0f);
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position + breathOffset, breathSize);
    }
#endif
}
