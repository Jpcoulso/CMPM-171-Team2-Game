using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    public enum CharacterState
    {
        Idle,
        Moving,
        Chasing,
        Attacking
    }

    public CharacterState CurrentState { get; private set; } = CharacterState.Idle;


    protected float currentHealth; // protected means only this class and subclasses can read/change these values
    protected Character currentTarget;
    protected bool isDead;


    protected float attackCooldown = 0;
    protected Rigidbody2D rb;
    protected Animator animator;
    

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public Character Target => currentTarget;

    // When true, the state machine is paused (used during knockback, stuns, etc.)
    [HideInInspector] public bool isKnockedBack = false;

    // Damage multiplier applied when this character takes damage (used by Magic Circle, etc.)
    [HideInInspector] public float incomingDamageMultiplier = 1f;

    // Shield state — set by ShieldEffect, checked by TakeDamage
    [HideInInspector] public bool shieldActive = false;
    [HideInInspector] public Vector2 shieldDirection;
    [HideInInspector] public float shieldHalfAngle;

    // Invulnerability — set by UnbreakableEffect, blocks ALL damage
    [HideInInspector] public bool isInvulnerable = false;

    // Movement speed multiplier — 1.0 = normal, lower = slower (used by slow effects)
    [HideInInspector] public float moveSpeedMultiplier = 1f;



    // abstract properties, when a subclass inherits from this class they MUST fill these in
    public abstract float MaxHealth {get;}
    public abstract float AttackDamage {get;}
    public abstract float MoveSpeed {get;}
    public abstract float AttackRange {get;}
    public abstract float AttackRate {get;}
    public abstract bool IsRanged {get;}
    public abstract bool IsHealer {get;}
    public abstract GameObject ProjectilePrefab {get;}
    protected Vector3 moveDestination;
    protected bool hasDestination;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    protected virtual void FixedUpdate()
    {
        if (isKnockedBack) return; // pause state machine during knockback
        UpdateState();
        ExcecuteState();
    }

    private void UpdateState()
    {
        switch (CurrentState)
        {
            case CharacterState.Idle:
                if(currentTarget != null)
                {
                    TransitionToState(CharacterState.Chasing);
                }
                else if(hasDestination == true)
                {
                    TransitionToState(CharacterState.Moving);
                }
                break;
            case CharacterState.Moving:
                if (HasReachedDestination())
                {
                    hasDestination = false;
                    TransitionToState(CharacterState.Idle);
                }
                break;
            case CharacterState.Chasing:
                if(currentTarget == null)
                {
                    TransitionToState(CharacterState.Idle);
                }
                else if(IsWithinAttackRange())
                {
                    TransitionToState(CharacterState.Attacking);
                }
                else if(hasDestination == true)
                {
                    TransitionToState(CharacterState.Moving);
                    currentTarget = null;
                }
                break;
            case CharacterState.Attacking:
                if(currentTarget == null)
                {
                    TransitionToState(CharacterState.Idle);
                }
                else if (!IsWithinAttackRange())
                {
                    TransitionToState(CharacterState.Chasing);
                }
            break;
        }
    }
    private void ExcecuteState()
    {
        switch (CurrentState)
        {
            case CharacterState.Idle:
                animator.SetBool("isWalking", false);
                //animator.SetBool("isAttacking", false);
                // Do nothing for now
                break;
            case CharacterState.Moving:
                animator.SetBool("isWalking", true);
                MoveTowards(moveDestination);
                break;
            case CharacterState.Chasing:
                animator.SetBool("isWalking", true);
                Vector3 engagementPoint = CalcEngagementPoint(currentTarget.transform.position);
                MoveTowards(engagementPoint);
                break;
            case CharacterState.Attacking:
                animator.SetBool("isWalking", false);
                //animator.SetBool("isAttacking", true);
                FaceTarget(currentTarget.transform.position);
                TryAttack();
                break;
        }
    }
    private void TransitionToState(CharacterState newState)
    {
        CurrentState = newState;
    }

    protected virtual void MoveTowards(Vector3 position)
    {
        FaceTarget(position);
        Vector2 direction = ((Vector2)position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * MoveSpeed * moveSpeedMultiplier * Time.fixedDeltaTime);
    }
    protected virtual bool HasReachedDestination()
    {
        return Vector2.Distance(transform.position, moveDestination) < 0.1f;
    }



    //------------shared properties, everything that inherits from Character.cs will use these properties---------------

    
    public void SetTarget(Character target) // Sets target, sets characterState to chasing
    {
        hasDestination = false;
        currentTarget = target;
        CurrentState = CharacterState.Chasing;
        Debug.Log("Target set!!!");
    }

    public void ClearTarget()
    {
        currentTarget = null;
        CurrentState = CharacterState.Idle;
        Debug.Log("ClearTarget() called: target = null, state = idle");
    }

    public void SetDestination(Vector3 destination)
    {
        currentTarget = null;
        moveDestination = destination;
        hasDestination = true;
        Debug.Log("Destination set");
    }

    private bool IsWithinAttackRange()
    {
        if (currentTarget == null) return false;
        if (IsRanged)
        {
            float distance = Vector2.Distance(transform.position,
                                              currentTarget.transform.position);
            return distance <= AttackRange;
        }
        return Mathf.Abs(transform.position.y - currentTarget.transform.position.y) <= 0.2f && Mathf.Abs(transform.position.x - currentTarget.transform.position.x) <= AttackRange;
        
    }

    public void TryAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0 && currentTarget != null && IsWithinAttackRange())
        {
            PerformAttack();
            if(attackCooldown <= 0) attackCooldown = AttackRate; // Reset cooldown if it has not been reset by anything else already (inheritance subclasses have custom resets)
        }
    }

    public virtual void FaceTarget(Vector3 position)
    {
        if (position.x < transform.position.x)
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face left (default)
    else
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face right (flipped)

    }
    
    protected virtual void PerformAttack()
    {
        Debug.Log($"{GetCharacterName()} attacks {Target.GetCharacterName()} for {AttackDamage} damage!");
        animator.SetTrigger("Attack"); // Target.TakeDamage(AttackDamage) gets called from Attack animation event
    }

    // if character is a ranged class this spawns a projectile and then Projectile.cs handles aiming, firing, and damage
    // else if character is a melee class OnAttackImpact() calls TakeDamage() like usual
    public void OnAttackImpact()
    {
        if(IsRanged && ProjectilePrefab != null)
        {
            Vector3 spawnPoint = transform.position;
            GameObject projectileObj = Instantiate(ProjectilePrefab, spawnPoint, quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if(projectile != null)
            {
                projectile.Initialize(currentTarget, AttackDamage, this);
            }
        }
        else
        {
            currentTarget.TakeDamage(AttackDamage, this);
        }
    }

    // Non-directional damage (abilities, AOE, etc.) — bypasses shield
    public virtual void TakeDamage(float rawAmount)
    {
        ApplyDamage(rawAmount);
    }

    // Directional damage (melee attacks) — can be blocked by shield
    public virtual void TakeDamage(float rawAmount, Character attacker)
    {
        if (isDead) return;

        // Check if shield blocks this attack based on direction
        if (shieldActive)
        {
            Vector2 toSource = ((Vector2)(attacker.transform.position - transform.position)).normalized;
            float angle = Vector2.Angle(shieldDirection, toSource);
            if (angle <= shieldHalfAngle)
            {
                Debug.Log(GetCharacterName() + " blocked damage with shield!");
                return;
            }
        }

        ApplyDamage(rawAmount);

        // change aggro to attacker if their armor class is higher than currentTarget
        if (currentTarget == null || currentTarget.GetArmor() < attacker.GetArmor())
        {
            SetTarget(attacker);
        }
    }

    private void ApplyDamage(float rawAmount)
    {
        if (isDead) return;
        if (isInvulnerable) return;

        // Apply any incoming damage multipliers (e.g. Magic Circle empowerment)
        float amplifiedAmount = rawAmount * incomingDamageMultiplier;

        // Armor reduces incoming damage, minimum of 5
        float reducedDamage = Mathf.Max(5, amplifiedAmount - GetArmor());

        currentHealth -= reducedDamage;

        Debug.Log($"{GetCharacterName()} took {reducedDamage} damage. " +
                  $"HP: {currentHealth}/{MaxHealth}");

        OnDamageTaken(reducedDamage);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
    }

    protected void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{GetCharacterName()} has died.");
        animator.SetTrigger("Died");
        OnDeath();
    }

    protected virtual Vector3 CalcEngagementPoint(Vector3 targetPosition)
    {
        float targetOffset = AttackRange * 0.5f;
        if(transform.position.x < targetPosition.x)
        {
            return new Vector3(targetPosition.x - targetOffset, targetPosition.y, 0);
        }
        return new Vector3(targetPosition.x + targetOffset, targetPosition.y, 0);
    }
    
    protected virtual void OnDamageTaken(float amount)          { }
    protected virtual void OnDeath()                            { }

    // Subclasses can override these to return
    // their own armor value or name
    public virtual float  GetArmor()         => 0f;
    public virtual string GetCharacterName() => "Unknown";
}
