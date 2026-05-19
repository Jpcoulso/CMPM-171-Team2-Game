using UnityEngine;
using UnityEngine.InputSystem;

// TestRedCircleAbility.cs
// A simple test ability that spawns a red circle wherever the mouse cursor is.
// In Unity: Right Click in Assets -> Create -> RPG -> Abilities -> Test Red Circle
// Create TWO assets from this (e.g. TestQ.asset and TestW.asset),
// then drag them into your hero's HeroData abilities list at slots 0 and 1.

[CreateAssetMenu(fileName = "TestRedCircle", menuName = "RPG/Abilities/Test Red Circle")]
public class TestRedCircleAbility : AbilityData
{
    [Header("Test Settings")]
    public float circleSize = 2f;   // world-unit diameter, roughly player-sized
    public float lifetime = 1.5f;

    public override void Execute(Character owner)
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;

        Debug.Log(owner.GetCharacterName() + " uses " + abilityName + " at " + worldPos);

        // Build a big red circle at runtime — no sprite asset needed
        GameObject circle = new GameObject("AbilityCircle_" + abilityName);
        circle.transform.position = worldPos;

        SpriteRenderer sr = circle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(64);
        sr.color = Color.red;
        sr.sortingLayerName = "Foreground";  // topmost sorting layer in this project
        sr.sortingOrder = 999;

        // The sprite is 64px with pixelsPerUnit=64, so 1 unit wide by default.
        // Scale it up to match circleSize.
        circle.transform.localScale = Vector3.one * circleSize;

        Object.Destroy(circle, lifetime);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive — nothing to do
    }

    private Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = size * 0.5f;
        float radiusSq = center * center;

        Color[] pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center + 0.5f;
                float dy = y - center + 0.5f;
                pixels[y * size + x] = (dx * dx + dy * dy <= radiusSq) ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Bilinear;
        tex.Apply();

        // pixelsPerUnit = size so the sprite is exactly 1 world unit before scaling
        return Sprite.Create(
            tex,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size
        );
    }
}
