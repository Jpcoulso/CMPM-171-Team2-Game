using UnityEngine;

// AbilityVFX.cs — Spawns simple visual effects for abilities.
// All VFX fade out and self-destruct after their duration.

public static class AbilityVFX
{
    private static Sprite squareSprite;

    // Spawns a colored square that fades out over the given duration.
    public static GameObject SpawnSquare(Vector3 position, float size, Color color, float duration)
    {
        var go = new GameObject("AbilityVFX");
        go.transform.position = position;
        go.transform.localScale = new Vector3(size, size, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = color;
        sr.sortingOrder = 5;

        go.AddComponent<VFXFader>().Initialize(duration, color);

        return go;
    }

    // Spawns a square that expands outward while fading (looks like a shockwave ring).
    public static GameObject SpawnRing(Vector3 center, float startSize, float endSize, Color color, float duration)
    {
        var go = new GameObject("AbilityRingVFX");
        go.transform.position = center;
        go.transform.localScale = new Vector3(startSize, startSize, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = color;
        sr.sortingOrder = 5;

        go.AddComponent<VFXRingExpand>().Initialize(startSize, endSize, duration, color);

        return go;
    }

    // Generates a simple 4x4 white square sprite at runtime (cached).
    private static Sprite GetSquareSprite()
    {
        if (squareSprite != null) return squareSprite;

        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Point;

        squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        return squareSprite;
    }
}

// Fades a SpriteRenderer's alpha to 0 over time, then destroys the GameObject.
public class VFXFader : MonoBehaviour
{
    private float duration;
    private float timer;
    private Color startColor;
    private SpriteRenderer sr;

    public void Initialize(float duration, Color startColor)
    {
        this.duration = duration;
        this.startColor = startColor;
    }

    void Start() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        if (t >= 1f) { Destroy(gameObject); return; }

        Color c = startColor;
        c.a = startColor.a * (1f - t);
        sr.color = c;
    }
}

// Scales a square up from startSize to endSize while fading out (shockwave effect).
public class VFXRingExpand : MonoBehaviour
{
    private float startSize, endSize, duration;
    private float timer;
    private Color startColor;
    private SpriteRenderer sr;

    public void Initialize(float startSize, float endSize, float duration, Color startColor)
    {
        this.startSize = startSize;
        this.endSize = endSize;
        this.duration = duration;
        this.startColor = startColor;
    }

    void Start() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        if (t >= 1f) { Destroy(gameObject); return; }

        float size = Mathf.Lerp(startSize, endSize, t);
        transform.localScale = new Vector3(size, size, 1f);

        Color c = startColor;
        c.a = startColor.a * (1f - t);
        sr.color = c;
    }
}
