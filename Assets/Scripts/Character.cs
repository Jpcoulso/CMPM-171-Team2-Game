using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected float currentHealth; // protected means only this class and subclasses can read/change these values
    protected bool isDead;


    // abstract properties, when a subclass inherits from this class they MUST fill these in
    public abstract float MaxHealth {get;}
    public abstract float AttackDamage {get;}
    public abstract float MoveSpeed {get;}


    //shared properties, everything that inherits from Character.cs will use these properties
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

    

    protected virtual void OnDamageTaken(float amount) { }
    protected virtual void OnDeath()                   { }

    // Subclasses can override these to return
    // their own armor value or name
    public virtual float  GetArmor()         => 0f;
    public virtual string GetCharacterName() => "Unknown";
}
