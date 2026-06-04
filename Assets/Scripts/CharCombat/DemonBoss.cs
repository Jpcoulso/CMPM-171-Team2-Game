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
    [SerializeField] private Vector2 breathSize = new Vector2(4f, 2f);
    [SerializeField] private GameObject breathVFXPrefab;
 
    // Convenience list for cooldown ticking
    private List<BossAttack> allAttacks;
 
    // The attack currently being executed — AnimationEventRelay uses this
    private BossAttack activeAttack;
 
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
 
    // Priority order: Fireball (long range) → Breath or Jump (close range) → Cleave (default).
    private BossAttack SelectAttack(float distance)
    {
        return fireballAttack;
        /* --- DEBUGGING
        // 1. Fireball — prefer at long range
        if (distance > cleaveAttack.range && distance <= fireballAttack.range && IsReady(fireballAttack))
            return fireballAttack;
 
        // 2. Close-range specials — pick whichever is off cooldown (breath wins the tie)
        if (distance <= breathAttack.range && IsReady(breathAttack))
            return breathAttack;
 
        if (distance <= jumpAttack.range && IsReady(jumpAttack))
            return jumpAttack;
 
        // 3. Cleave — default melee fallback
        if (distance <= cleaveAttack.range && IsReady(cleaveAttack))
            return cleaveAttack;
 
        return null; // nothing is in range or off cooldown yet
        */
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
 
    // Spawns a projectile that flies toward the current target
    public void OnFireballLaunch()
    {
        if (currentTarget == null || ProjectilePrefab == null) return;
 
        Vector3 spawnPosition = transform.position; //firePoint != null ? firePoint.position : transform.position;
 
        GameObject projectileObj = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
 
        if (projectile != null)
            projectile.Initialize(currentTarget, (int)fireballAttack.damage, this);
        else
            Debug.LogWarning($"{name}: ProjectilePrefab is missing a Projectile component!");
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