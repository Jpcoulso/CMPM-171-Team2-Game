using UnityEngine;

public class Fireball : Projectile
{
    protected Animator animator;
    private bool hasImpacted = false;
    public override void Initialize(Character target, float damage, Character attacker)
    {
        this.target   = target;
        this.damage   = damage;
        this.attacker = attacker;
        animator = GetComponent<Animator>();
        // Face the target on spawn
        AimAtTarget();
    }

    protected override void Update()
    {
        if(hasImpacted) return;
        base.Update();
    }

    protected override void OnImpact()
    {
        hasImpacted = true;
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
        animator.SetTrigger("explode"); // explode animation calls destory(GameObject) after it has played
    }
}
