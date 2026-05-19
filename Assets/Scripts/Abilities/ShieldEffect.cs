using UnityEngine;

// Attached to the shield visual GameObject. Follows the owner,
// sets shield fields on Character so TakeDamage can block directional attacks.

public class ShieldEffect : MonoBehaviour
{
    private Character owner;
    private Vector2 shieldDir;
    private float halfArc;
    private float duration;
    private float timer;
    private float offset;
    private SpriteRenderer cachedSR;

    public void Initialize(Character shieldOwner, Vector2 direction, float arcDegrees, float shieldDuration, float shieldOffset)
    {
        owner = shieldOwner;
        shieldDir = direction.normalized;
        halfArc = arcDegrees * 0.5f;
        duration = shieldDuration;
        offset = shieldOffset;
        timer = 0f;
        cachedSR = GetComponent<SpriteRenderer>();

        // Tell the Character it has an active shield
        owner.shieldActive = true;
        owner.shieldDirection = shieldDir;
        owner.shieldHalfAngle = halfArc;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration || owner == null || owner.IsDead)
        {
            Expire();
            return;
        }

        // Follow the owner — stay offset in the shield direction
        transform.position = owner.transform.position + (Vector3)(shieldDir * offset);

        // Pulse the shield slightly so it looks active
        if (cachedSR != null)
        {
            float alpha = 0.55f + 0.2f * Mathf.Sin(timer * 5f);
            Color c = cachedSR.color;
            cachedSR.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    private void Expire()
    {
        if (owner != null)
            owner.shieldActive = false;

        Debug.Log("Shield expired.");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Safety: always clear shield state
        if (owner != null)
            owner.shieldActive = false;
    }
}
