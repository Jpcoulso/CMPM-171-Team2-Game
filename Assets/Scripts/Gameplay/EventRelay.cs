using UnityEngine;
// This script sits on the child (same object as the Animator)
public class AnimationEventRelay : MonoBehaviour
{
    private Character parent;

    void Start()
    {
        // Walk up to the parent to find the logic script
        parent = GetComponentInParent<Character>();
    }

    // Animation Event calls this — passes attacker position so shields can block directionally
    void OnAttackLand()
    {
        parent.OnAttackImpact();
    }

    void OnDeath()
    {
        //await Task.Delay(2000);
        Destroy(transform.parent.gameObject);
    }

    // used to destroy fireball after it explodes
    void DestroyMe()
    {
        Destroy(gameObject);
    }

     // --- Boss Attack Relays ---
    void OnCleaveImpact()
    {
        if (parent is DemonBoss boss) boss.OnCleaveImpact();
    }
    
    void OnFireballLaunch()
    {
        if (parent is DemonBoss boss) boss.OnFireballLaunch();
    }
   
    void OnJumpImpact()
    {
        if (parent is DemonBoss boss) boss.OnJumpImpact();
    }
   
    void OnBreathImpact()
    {
        if (parent is DemonBoss boss) boss.OnBreathImpact();
    }
}
