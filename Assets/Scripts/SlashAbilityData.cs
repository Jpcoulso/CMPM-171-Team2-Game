using UnityEngine;
using UnityEngine.InputSystem;

// SlashAbilityData.cs
// A crescent arc slash around the hero toward the cursor direction.
// Damages all enemies within the arc. Visual is a procedural crescent shape.

[CreateAssetMenu(fileName = "Slash", menuName = "RPG/Abilities/Slash")]
public class SlashAbilityData : AbilityData
{
    [Header("Slash Settings")]
    public float damageMultiplier = 3f;
    public float slashRadius = 1.8f;
    public float innerRadius = 0.8f;
    public float arcAngle = 120f;
    public float visualDuration = 0.35f;

    public override void Execute(Character owner)
    {
        // Get cursor world position to determine slash direction
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 heroPos = owner.transform.position;

        // Direction from hero toward cursor
        Vector2 cursorDir = ((Vector2)(mouseWorld - heroPos)).normalized;
        float cursorAngle = Mathf.Atan2(cursorDir.y, cursorDir.x) * Mathf.Rad2Deg;

        // Calculate damage
        float damage = owner.AttackDamage * damageMultiplier;
        float halfArc = arcAngle * 0.5f;

        Debug.Log(owner.GetCharacterName() + " uses Slash! Arc toward cursor, dealing " + damage + " damage.");

        // Find all enemies within the outer radius, then filter by arc angle
        Collider2D[] hits = Physics2D.OverlapCircleAll(heroPos, slashRadius);

        int enemiesHit = 0;
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead) continue;

            Vector2 toEnemy = ((Vector2)(enemy.transform.position - heroPos));
            float dist = toEnemy.magnitude;

            // Skip enemies inside the inner radius
            if (dist < innerRadius) continue;

            // Check if enemy is within the arc angle
            float enemyAngle = Mathf.Atan2(toEnemy.y, toEnemy.x) * Mathf.Rad2Deg;
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(cursorAngle, enemyAngle));

            if (angleDiff <= halfArc)
            {
                enemy.TakeDamage(damage);
                enemiesHit++;
                Debug.Log("Slash hit " + enemy.GetCharacterName() + " for " + damage + " damage!");
            }
        }

        if (enemiesHit == 0)
        {
            Debug.Log("Slash hit no enemies.");
        }

        // Spawn crescent visual centered on hero, rotated toward cursor
        SpawnCrescentVisual(heroPos, cursorAngle);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }

    private void SpawnCrescentVisual(Vector3 heroPos, float angleDeg)
    {
        GameObject visual = new GameObject("SlashArc_Visual");
        visual.transform.position = heroPos;

        // Rotate the visual so the arc faces toward the cursor
        visual.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetCrescent(64, innerRadius / slashRadius, arcAngle * 0.5f);
        sr.color = new Color(1f, 0.25f, 0.2f, 0.65f); // semi-transparent red-orange
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 999;

        float diameter = slashRadius * 2f;
        visual.transform.localScale = new Vector3(diameter, diameter, 1f);

        Object.Destroy(visual, visualDuration);
    }

}
