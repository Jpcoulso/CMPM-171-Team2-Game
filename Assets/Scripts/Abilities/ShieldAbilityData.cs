using UnityEngine;
using UnityEngine.InputSystem;

// Spawns a thin blue barrier in front of the hero toward the cursor direction.
// Blocks incoming melee damage from that direction for 2 seconds. 5 second cooldown.

[CreateAssetMenu(fileName = "ShieldUp", menuName = "RPG/Abilities/Shield Up")]
public class ShieldAbilityData : AbilityData
{
    [Header("Shield Settings")]
    public float shieldDuration = 2f;
    public float shieldWidth = 2f;
    public float shieldThickness = 0.12f;
    public float shieldArc = 120f;
    public float shieldOffset = 0.8f;

    public override void Execute(Character owner)
    {
        // Get cursor direction to determine shield facing
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = ((Vector2)(mouseWorld - owner.transform.position)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Debug.Log(owner.GetCharacterName() + " raises Shield! Blocking for " + shieldDuration + "s.");

        // Create the shield barrier visual
        GameObject shield = new GameObject("ShieldUp_Visual");
        Vector3 shieldPos = owner.transform.position + (Vector3)(dir * shieldOffset);
        shield.transform.position = shieldPos;
        // Rotate so the thin edge faces the cursor direction (barrier is perpendicular)
        shield.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        SpriteRenderer sr = shield.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetRect(64, 64);
        sr.color = new Color(0.3f, 0.75f, 1f, 0.65f); // light blue 
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 999;

        shield.transform.localScale = new Vector3(shieldThickness, shieldWidth, 1f);

        // Add the effect component that handles blocking + following
        ShieldEffect effect = shield.AddComponent<ShieldEffect>();
        effect.Initialize(owner, dir, shieldArc, shieldDuration, shieldOffset);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }
}
