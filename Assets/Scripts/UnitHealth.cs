using UnityEngine;

// Simple health component for player units.
// Attach to any unit that should display health in the HUD.
// Enemies call TakeDamage() on this when they attack.
public class UnitHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercent => maxHealth > 0 ? currentHealth / maxHealth : 0f;
    public bool IsAlive => currentHealth > 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        Debug.Log($"[{gameObject.name}] Took {damage} damage! HP: {currentHealth}/{maxHealth}");

        if (!IsAlive)
        {
            OnDeath();
        }
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"[{gameObject.name}] Healed {amount}! HP: {currentHealth}/{maxHealth}");
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void OnDeath()
    {
        Debug.Log($"[{gameObject.name}] has been defeated!");
        // TODO: Add death logic (disable unit, play animation, etc.)
    }
}
