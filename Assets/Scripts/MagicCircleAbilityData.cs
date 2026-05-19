using UnityEngine;
using UnityEngine.InputSystem;

// MagicCircleAbilityData.cs
// Spawns a purple circle zone at the cursor position.
// Enemies inside take 2x damage for 10 seconds. 30 second cooldown.

[CreateAssetMenu(fileName = "MagicCircle", menuName = "RPG/Abilities/Magic Circle")]
public class MagicCircleAbilityData : AbilityData
{
    [Header("Magic Circle Settings")]
    public float circleRadius = 2.5f;
    public float damageMultiplier = 2f;
    public float circleDuration = 10f;

    public override void Execute(Character owner)
    {
        // Spawn the circle at the cursor position
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;

        Debug.Log(owner.GetCharacterName() + " casts Magic Circle! " + damageMultiplier + "x damage for " + circleDuration + "s.");

        // Create the circle GameObject with visual and effect
        GameObject circle = new GameObject("MagicCircle");
        circle.transform.position = worldPos;

        // Add the purple ring visual
        SpriteRenderer sr = circle.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetRing(64, 0.9f);
        sr.color = new Color(0.6f, 0.2f, 0.9f, 0.5f); // purple
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 500;

        // Scale to match the circle radius
        float diameter = circleRadius * 2f;
        circle.transform.localScale = new Vector3(diameter, diameter, 1f);

        // Add the effect component that handles empowerment logic
        MagicCircleEffect effect = circle.AddComponent<MagicCircleEffect>();
        effect.Initialize(circleRadius, damageMultiplier, circleDuration);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }

}
