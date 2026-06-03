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
}
