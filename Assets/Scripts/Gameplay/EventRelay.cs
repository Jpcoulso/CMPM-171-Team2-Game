using System.Threading.Tasks;
using Unity.VisualScripting;
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

    // Animation Event calls this
    void OnAttackLand()
    {
        parent.Target.TakeDamage(parent.AttackDamage);
    }

    void OnDeath()
    {
        //await Task.Delay(2000);
        Destroy(transform.parent.gameObject);
    }
}
