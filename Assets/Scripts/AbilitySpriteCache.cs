using UnityEngine;
using System.Collections.Generic;

// AbilitySpriteCache.cs
// Shared sprite cache for all abilities. Each shape is generated once and reused.
// Prevents memory leaks from creating new Texture2D/Sprite every ability cast.

public static class AbilitySpriteCache
{
    private static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite GetCircle(int size)
    {
        string key = "circle_" + size;
        if (cache.ContainsKey(key)) return cache[key];

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        float center = size * 0.5f;
        float radiusSq = center * center;

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

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        cache[key] = sprite;
        return sprite;
    }

    public static Sprite GetRing(int size, float innerRatio)
    {
        string key = "ring_" + size + "_" + innerRatio.ToString("F2");
        if (cache.ContainsKey(key)) return cache[key];

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        float center = size * 0.5f;
        float outerR = center;
        float innerR = center * innerRatio;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center + 0.5f;
                float dy = y - center + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                bool inRing = dist >= innerR && dist <= outerR;
                pixels[y * size + x] = inRing ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Bilinear;
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        cache[key] = sprite;
        return sprite;
    }

    public static Sprite GetRect(int width, int height)
    {
        string key = "rect_" + width + "x" + height;
        if (cache.ContainsKey(key)) return cache[key];

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), width);
        cache[key] = sprite;
        return sprite;
    }

    public static Sprite GetCrescent(int size, float innerRatio, float halfArcDeg)
    {
        string key = "crescent_" + size + "_" + innerRatio.ToString("F2") + "_" + halfArcDeg.ToString("F1");
        if (cache.ContainsKey(key)) return cache[key];

        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        float center = size * 0.5f;
        float outerR = center;
        float innerR = center * innerRatio;
        float halfArcRad = halfArcDeg * Mathf.Deg2Rad;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center + 0.5f;
                float dy = y - center + 0.5f;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                bool inRing = dist >= innerR && dist <= outerR;
                float pixelAngle = Mathf.Abs(Mathf.Atan2(dy, dx));
                bool inArc = pixelAngle <= halfArcRad;
                pixels[y * size + x] = (inRing && inArc) ? Color.white : Color.clear;
            }
        }

        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Bilinear;
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        cache[key] = sprite;
        return sprite;
    }
}
