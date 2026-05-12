using UnityEngine;

// AbilityHitbox.cs — Instant box-shaped damage zone.
// Spawned by abilities at a position+scale, damages everything inside once, then self-destructs.
// Uses Physics2D.OverlapBoxAll so no collider is needed on this GameObject.

public class AbilityHitbox : MonoBehaviour
{
    private float damage;
    private float lifetime;
    private GameObject caster;
    private bool hasDamaged = false;

    public void Initialize(float damage, float lifetime, GameObject caster)
    {
        this.damage = damage;
        this.lifetime = lifetime;
        this.caster = caster;
    }

    void Start()
    {
        DealDamage();
        Destroy(gameObject, lifetime);
    }

    void DealDamage()
    {
        if (hasDamaged) return;
        hasDamaged = true;

        Vector2 size = new Vector2(transform.localScale.x, transform.localScale.y);
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, size, 0f);

        foreach (var hit in hits)
        {
            if (hit.gameObject == caster) continue;

            Character target = hit.GetComponent<Character>();
            if (target != null && !target.IsDead)
                target.TakeDamage(damage);
        }
    }
}
