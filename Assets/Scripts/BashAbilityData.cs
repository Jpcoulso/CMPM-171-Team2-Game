using UnityEngine;
using UnityEngine.InputSystem;

// BashAbilityData.cs
// A rectangular AOE bash toward the cursor that deals moderate damage and knocks enemies back.

[CreateAssetMenu(fileName = "Bash", menuName = "RPG/Abilities/Bash")]
public class BashAbilityData : AbilityData
{
    [Header("Bash Settings")]
    public float damageMultiplier = 1.5f;
    public float rectWidth = 1.2f;
    public float rectLength = 2f;
    public float offsetDistance = 0.6f;
    public float knockbackDistance = 3f;   
    public float knockbackDuration = 0.25f;
    public float visualDuration = 0.3f;

    public override void Execute(Character owner)
    {
        // Get cursor direction
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 heroPos = owner.transform.position;
        Vector2 dir = ((Vector2)(mouseWorld - heroPos)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Center of the rectangle is offset along the cursor direction
        Vector2 rectCenter = (Vector2)heroPos + dir * (offsetDistance + rectLength * 0.5f);

        float damage = owner.AttackDamage * damageMultiplier;

        Debug.Log(owner.GetCharacterName() + " uses Bash! Dealing " + damage + " damage with knockback.");

        // OverlapBox needs the half-extents and rotation angle
        // rectLength is along the cursor direction, rectWidth is perpendicular
        Vector2 halfExtents = new Vector2(rectLength * 0.5f, rectWidth * 0.5f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(rectCenter, halfExtents * 2f, angle);

        int enemiesHit = 0;
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead) continue;

            // Deal damage
            enemy.TakeDamage(damage);
            enemiesHit++;

            // Apply smooth knockback — add a KnockbackEffect component that slides them back
            Vector2 knockDir = ((Vector2)(enemy.transform.position - heroPos)).normalized;
            KnockbackEffect kb = enemy.gameObject.AddComponent<KnockbackEffect>();
            kb.Initialize(knockDir, knockbackDistance, knockbackDuration);

            Debug.Log("Bash hit " + enemy.GetCharacterName() + " for " + damage + " damage + knockback!");
        }

        if (enemiesHit == 0)
        {
            Debug.Log("Bash hit no enemies.");
        }

        // Spawn the rectangle visual
        SpawnBashVisual(rectCenter, angle);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }

    private void SpawnBashVisual(Vector2 center, float angleDeg)
    {
        GameObject visual = new GameObject("BashAOE_Visual");
        visual.transform.position = new Vector3(center.x, center.y, 0f);
        visual.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetRect(64, 32); // wider than tall, rotated to face cursor
        sr.color = new Color(1f, 0.6f, 0.15f, 0.65f); // semi-transparent orange
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 999;

        visual.transform.localScale = new Vector3(rectLength, rectWidth, 1f);

        Object.Destroy(visual, visualDuration);
    }

}
