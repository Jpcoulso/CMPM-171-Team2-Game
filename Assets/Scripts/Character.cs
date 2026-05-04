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
                if()
                break;
            case CharacterState.Attacking:
            break;
        }
    }
    private void ExcecuteState(){}
    private void TransitionToState(CharacterState newState)
    {
        CurrentState = newState;
    }

    protected abstract void MoveTowards(Vector3 position);
    protected abstract void MoveToDestination();
    protected abstract void FaceTarget(Vector3 position);
    protected abstract bool HasReachedDestination();




    //shared properties, everything that inherits from Character.cs will use these properties
    public void SetTarget(Character target)
    {
        currentTarget = target;
        //,hasDestination = false;
        Debug.Log("Target set!!!");
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
