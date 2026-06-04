using UnityEngine;
 
public class Projectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float speed = 10f;
 
    // --- Runtime State ---
    protected Character target;
    protected float damage;
    protected Character attacker;
    protected const float IMPACT_DISTANCE = 0.2f;
 
    // Called immediately after Instantiate to give the projectile everything it needs.
    public virtual void Initialize(Character target, float damage, Character attacker)
    {
        this.target   = target;
        this.damage   = damage;
        this.attacker = attacker;
 
        // Face the target on spawn
        AimAtTarget();
    }
 
    protected virtual void Update()
    {
        // --- Cleanup: target died or was destroyed before impact ---
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }
 
        MoveTowardsTarget();
        CheckImpact();
    }
 
    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------
 
    protected void MoveTowardsTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
 
        // Keep the sprite/arrow pointing at the target each frame
        AimAtTarget();
    }
 
    protected void AimAtTarget()
    {
        if (target == null) return;
 
        Vector3 direction = target.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
 
    // -------------------------------------------------------------------------
    // Impact
    // -------------------------------------------------------------------------
 
    protected void CheckImpact()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
 
        if (distanceToTarget < IMPACT_DISTANCE)
        {
            OnImpact();
        }
    }
 
    protected virtual void OnImpact()
    {
        if (attacker.IsHealer)
        {
            target.Heal(damage);
        }
        else
        {
            target.TakeDamage(damage, attacker);
        }
        
        // Optional: spawn a hit VFX here before destroying
        // Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}