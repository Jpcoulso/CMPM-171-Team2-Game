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


    private float attackCooldown = 0;
    private Rigidbody2D rb;
    private Animator animator;
    
    // --- Buff state ---
    private float shieldAmount;
    private float shieldTimer;
    private float damageMultiplier = 1f;
    private float damageMultiplierTimer;
    private GameObject shieldBorderObj;
    private GameObject rageBorderObj;

    public float CurrentHealth => currentHealth;
    public float ShieldAmount => shieldAmount;
    public bool IsDead => isDead;
    public Character Target => currentTarget;


    // abstract properties, when a subclass inherits from this class they MUST fill these in
    public abstract float MaxHealth {get;}
    public abstract float AttackDamage {get;}
    public abstract float MoveSpeed {get;}
    public abstract float AttackRange {get;}
    public abstract float AttackRate {get;}
    protected Vector3 moveDestination;
    protected bool hasDestination;

    // Subclasses can override these to return
    public virtual float  GetArmor()         => 0f;
    public virtual string GetCharacterName() => "Unknown";

    // --- Optional hooks for subclasses ---
    protected virtual void OnDamageTaken(float amount) { }
    protected virtual void OnDeath() { }
    protected virtual void AggroEnemy(Character targetEnemy)    { }

    // =============================================
    //  LIFECYCLE
    // =============================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }
    protected virtual void FixedUpdate()
    {
        UpdateState();
        ExcecuteState();
        TickBuffs();
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
                    Debug.Log("transitioned to attacking!!");
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
                MoveTowards(currentTarget.transform.position);
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
        rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
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
        float distance = Vector2.Distance(transform.position,
                                          currentTarget.transform.position);
        return distance <= AttackRange;
    }
    public void TryAttack()
    {
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0 && currentTarget != null && IsWithinAttackRange())
        {
            PerformAttack();
            attackCooldown = AttackRate; // Reset cooldown
        }
    }
    public void FaceTarget(Vector3 position)
    {
        if (position.x < transform.position.x)
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face left (default)
    else
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); // face right (flipped)

    }
    private void PerformAttack()
    {
        Debug.Log($"{GetCharacterName()} attacks {Target.GetCharacterName()} for {AttackDamage} damage!");
        animator.SetTrigger("Attack"); // Target.TakeDamage(AttackDamage) gets called from Attack animation event
        float finalDamage = AttackDamage * damageMultiplier;
        Target.TakeDamage(finalDamage);
    }

    public virtual void TakeDamage(float rawAmount)
    {
        if (isDead) return;

        // Armor reduces incoming damage, minimum of 1
        float damage = Mathf.Max(1, rawAmount - GetArmor());

    // Shield absorbs damage before health
        if (shieldAmount > 0f)
        {
            if (damage <= shieldAmount)
            {
                shieldAmount -= damage;
                return; // fully absorbed
            }
            else
            {
                damage -= shieldAmount;
                shieldAmount = 0f;
                RemoveBorder(ref shieldBorderObj);
            }
        }

        // Log for now — later this updates your UI health bar
        Debug.Log($"{GetCharacterName()} took {damage} damage. " +
                  $"HP: {currentHealth}/{MaxHealth}");

        OnDamageTaken(damage);

        if (currentHealth <= 0)
            Die();
    }

    protected void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{GetCharacterName()} has died.");
        animator.SetTrigger("Died");
        OnDeath();
    }

    // =============================================
    //  BUFF SYSTEM
    // =============================================
    // Grants a temporary shield that absorbs damage. Shows a white border.
    public void ApplyShield(float amount, float duration)
    {
        shieldAmount = amount;
        shieldTimer = duration;
        RemoveBorder(ref shieldBorderObj);
        shieldBorderObj = CreateBorder(new Color(1f, 1f, 1f, 0.7f), 1);
    }
    // Grants a temporary damage multiplier on auto-attacks. Shows a red border.
    public void ApplyDamageMultiplier(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        damageMultiplierTimer = duration;
        RemoveBorder(ref rageBorderObj);
        rageBorderObj = CreateBorder(new Color(1f, 0.15f, 0.1f, 0.7f), 2);
    }

    private void TickBuffs()
    {
        if (shieldTimer > 0f)
        {
            shieldTimer -= Time.fixedDeltaTime;
            if (shieldTimer <= 0f)
            {
                shieldAmount = 0f;
                shieldTimer = 0f;
                RemoveBorder(ref shieldBorderObj);
            }
        }

        if (damageMultiplierTimer > 0f)
        {
            damageMultiplierTimer -= Time.fixedDeltaTime;
            if (damageMultiplierTimer <= 0f)
            {
                damageMultiplier = 1f;
                damageMultiplierTimer = 0f;
                RemoveBorder(ref rageBorderObj);
            }
        }
    }

    // =============================================
    //  BORDER VISUALS (for buffs)
    // =============================================

    // Creates a colored square behind the character sprite to indicate a buff
    private GameObject CreateBorder(Color color, int sortOffset)
    {
        var border = new GameObject("BuffBorder");
        border.transform.SetParent(transform, false);
        border.transform.localPosition = Vector3.zero;

        SpriteRenderer charSR = GetComponent<SpriteRenderer>();
        float scaleX = charSR != null ? charSR.bounds.size.x + 0.3f : 1.3f;
        float scaleY = charSR != null ? charSR.bounds.size.y + 0.3f : 1.3f;
        border.transform.localScale = new Vector3(
            scaleX / Mathf.Abs(transform.localScale.x),
            scaleY / Mathf.Abs(transform.localScale.y),
            1f
        );

        var sr = border.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite();
        sr.color = color;
        sr.sortingOrder = (charSR != null ? charSR.sortingOrder : 0) - sortOffset;

        return border;
    }

    private void RemoveBorder(ref GameObject borderObj)
    {
        if (borderObj != null)
        {
            Destroy(borderObj);
            borderObj = null;
        }
    }

    // Generates a simple white 4x4 square sprite at runtime (cached)
    private static Sprite cachedSquare;
    private static Sprite CreateSquareSprite()
    {
        if (cachedSquare != null) return cachedSquare;
        var tex = new Texture2D(4, 4);
        var px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        cachedSquare = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        return cachedSquare;
    }
}
