using UnityEngine;

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


    private float attackCooldown;
    

    public bool IsDead => isDead;
    public Character Target => currentTarget;


    // abstract properties, when a subclass inherits from this class they MUST fill these in
    public abstract float MaxHealth {get;}
    public abstract float AttackDamage {get;}
    public abstract float MoveSpeed {get;}
    public abstract bool IsRanged {get;}

    public abstract float AttackRange {get;}
    public abstract float AttackRate {get;}
    protected Vector3 moveDestination;
    protected bool hasDestination;

    public virtual void Update()
    {
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
                break;
            case CharacterState.Moving:
                if (HasReachedDestination())
                {
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
                // Do nothing for now
                break;
            case CharacterState.Moving:
                MoveTowards(moveDestination);
                break;
            case CharacterState.Chasing:
                MoveTowards(currentTarget.transform.position);
                break;
            case CharacterState.Attacking:
                FaceTarget(currentTarget.transform.position);
                TryAttack();
                break;
        }
    }
    private void TransitionToState(CharacterState newState)
    {
        CurrentState = newState;
    }

    protected abstract void MoveTowards(Vector3 position);
    protected abstract void MoveToDestination();
    protected abstract bool HasReachedDestination();



    //------------shared properties, everything that inherits from Character.cs will use these properties---------------

    
    public void SetTarget(Character target) // Sets target, sets characterState to chasing
    {
        currentTarget = target;
        CurrentState = CharacterState.Chasing;
        //hasDestination = false;
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
        Target.TakeDamage(AttackDamage);
    }

    public virtual void TakeDamage(float rawAmount)
    {
        if (isDead) return;

        // Armor reduces incoming damage, minimum of 1
        float reducedDamage = Mathf.Max(1, rawAmount - GetArmor());

        currentHealth -= reducedDamage;

        // Log for now — later this updates your UI health bar
        Debug.Log($"{GetCharacterName()} took {reducedDamage} damage. " +
                  $"HP: {currentHealth}/{MaxHealth}");

        OnDamageTaken(reducedDamage);

        if (currentHealth <= 0)
            Die();
    }

    protected void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{GetCharacterName()} has died.");
        OnDeath();
    }

    
    protected virtual void OnDamageTaken(float amount)          { }
    protected virtual void OnDeath()                            { }
    protected virtual void AggroEnemy(Character targetEnemy)    { }

    // Subclasses can override these to return
    // their own armor value or name
    public virtual float  GetArmor()         => 0f;
    public virtual string GetCharacterName() => "Unknown";
}
