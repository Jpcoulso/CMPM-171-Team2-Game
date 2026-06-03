using System.Collections.Generic;
using UnityEngine;

// Visuals are generated procedurally — no art assets required.
public class AoeTelegraph : MonoBehaviour
{
    private enum Shape { Circle, Rectangle, Cone }

    private Shape shape;
    private float chargeTime;
    private float damage;
    private Character attacker;

    private float radius;
    private float length;
    private float width; 
    private float halfAngle; 
    private Vector2 dir = Vector2.right;

    private float elapsed;
    private bool resolved;
    private float fadeTimer;
    private const float LingerTime = 0.18f;
    private SpriteRenderer sr;

    private static readonly Color ChargeColor = new Color(1f, 0.35f, 0.1f, 0.28f);
    private static readonly Color PeakColor   = new Color(1f, 0.15f, 0.05f, 0.6f);
    private static readonly Color FlashColor  = new Color(1f, 1f, 1f, 0.85f);

    // Init API
    public void InitCircle(float radius, float chargeTime, float damage, Character attacker)
    {
        shape = Shape.Circle;
        this.radius = radius;
        this.chargeTime = chargeTime;
        this.damage = damage;
        this.attacker = attacker;
        BuildVisual();
    }

    public void InitRectangle(float length, float width, Vector2 dir, float chargeTime, float damage, Character attacker)
    {
        shape = Shape.Rectangle;
        this.length = length;
        this.width = width;
        this.dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        this.chargeTime = chargeTime;
        this.damage = damage;
        this.attacker = attacker;
        BuildVisual();
    }

    public void InitCone(float radius, float angle, Vector2 dir, float chargeTime, float damage, Character attacker)
    {
        shape = Shape.Cone;
        this.radius = radius;
        this.halfAngle = angle * 0.5f;
        this.dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        this.chargeTime = chargeTime;
        this.damage = damage;
        this.attacker = attacker;
        BuildVisual();
    }

    // A portion of this code is generated with Claude
    private void BuildVisual()
    {
        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(transform, false);

        sr = visual.AddComponent<SpriteRenderer>();
        sr.color = ChargeColor;
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 500;

        float dirAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        switch (shape)
        {
            case Shape.Circle:
                sr.sprite = TelegraphSprites.Circle();
                visual.transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
                break;

            case Shape.Rectangle:
                sr.sprite = TelegraphSprites.Rectangle(); // 1x1, pivot at left-center
                visual.transform.localRotation = Quaternion.Euler(0f, 0f, dirAngle);
                visual.transform.localScale = new Vector3(length, width, 1f);
                break;

            case Shape.Cone:
                sr.sprite = TelegraphSprites.Cone(halfAngle); // points +Y, apex at pivot
                visual.transform.localRotation = Quaternion.Euler(0f, 0f, dirAngle - 90f);
                visual.transform.localScale = new Vector3(radius, radius, 1f);
                break;
        }
    }

    private void Update()
    {
        if (!resolved)
        {
            elapsed += Time.deltaTime;
            float t = chargeTime > 0f ? Mathf.Clamp01(elapsed / chargeTime) : 1f;

            float pulse = 0.5f + 0.5f * Mathf.Sin(elapsed * (6f + 12f * t));
            Color c = Color.Lerp(ChargeColor, PeakColor, t);
            c.a *= 0.7f + 0.3f * pulse;
            if (sr != null) sr.color = c;

            if (elapsed >= chargeTime)
                Resolve();
        }
        else
        {
            fadeTimer += Time.deltaTime;
            if (sr != null)
            {
                Color c = FlashColor;
                c.a = Mathf.Lerp(FlashColor.a, 0f, fadeTimer / LingerTime);
                sr.color = c;
            }
            if (fadeTimer >= LingerTime)
                Destroy(gameObject);
        }
    }

    private void Resolve()
    {
        resolved = true;

        if (SquadManager.Instance != null)
        {
            IReadOnlyList<Hero> squad = SquadManager.Instance.GetSquad();
            foreach (Hero hero in squad)
            {
                if (hero == null || hero.IsDead) continue;
                if (IsInside(hero.transform.position))
                    hero.TakeDamage(damage);
            }
        }

        if (sr != null) sr.color = FlashColor;
    }

    private bool IsInside(Vector2 point)
    {
        Vector2 origin = transform.position;

        switch (shape)
        {
            case Shape.Circle:
                return Vector2.Distance(origin, point) <= radius;

            case Shape.Rectangle:
            {
                Vector2 local = point - origin;
                float forward = Vector2.Dot(local, dir);
                Vector2 perpDir = new Vector2(-dir.y, dir.x);
                float side = Vector2.Dot(local, perpDir);
                return forward >= 0f && forward <= length && Mathf.Abs(side) <= width * 0.5f;
            }

            case Shape.Cone:
            {
                Vector2 toPoint = point - origin;
                if (toPoint.magnitude > radius) return false;
                return Vector2.Angle(dir, toPoint) <= halfAngle;
            }
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        if (shape == Shape.Circle || shape == Shape.Cone)
            Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}

public static class TelegraphSprites
{
    private static Sprite _circle;
    private static Sprite _rectangle;
    private static readonly Dictionary<int, Sprite> _cones = new Dictionary<int, Sprite>();

    private const int Size = 128;

    public static Sprite Circle()
    {
        if (_circle != null) return _circle;

        Texture2D tex = NewTexture();
        Vector2 center = new Vector2(Size / 2f, Size / 2f);
        float r = Size / 2f - 2f;
        float ring = 4f;

        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            float d = Vector2.Distance(new Vector2(x, y), center);
            Color col = Color.clear;
            if (d <= r) col = new Color(1f, 1f, 1f, 0.55f);
            if (d <= r && d >= r - ring) col = Color.white;
            tex.SetPixel(x, y, col);
        }
        tex.Apply();

        _circle = MakeSprite(tex, new Vector2(0.5f, 0.5f));
        return _circle;
    }

    public static Sprite Rectangle()
    {
        if (_rectangle != null) return _rectangle;

        Texture2D tex = NewTexture(8, 8);
        for (int y = 0; y < 8; y++)
        for (int x = 0; x < 8; x++)
        {
            bool edge = x == 0 || x == 7 || y == 0 || y == 7;
            tex.SetPixel(x, y, edge ? Color.white : new Color(1f, 1f, 1f, 0.55f));
        }
        tex.Apply();

        _rectangle = MakeSprite(tex, new Vector2(0f, 0.5f));
        return _rectangle;
    }

    public static Sprite Cone(float halfAngleDeg)
    {
        int key = Mathf.RoundToInt(halfAngleDeg);
        if (_cones.TryGetValue(key, out Sprite cached)) return cached;

        Texture2D tex = NewTexture();
        Vector2 apex = new Vector2(Size / 2f, 0f);
        float r = Size - 2f;
        Vector2 up = Vector2.up;

        for (int y = 0; y < Size; y++)
        for (int x = 0; x < Size; x++)
        {
            Vector2 v = new Vector2(x, y) - apex;
            float d = v.magnitude;
            Color col = Color.clear;
            if (d <= r && Vector2.Angle(up, v) <= halfAngleDeg)
            {
                col = new Color(1f, 1f, 1f, 0.55f);
                if (d >= r - 5f) col = Color.white;
            }
            tex.SetPixel(x, y, col);
        }
        tex.Apply();

        Sprite s = MakeSprite(tex, new Vector2(0.5f, 0f));
        _cones[key] = s;
        return s;
    }

    private static Texture2D NewTexture(int w = Size, int h = Size)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }

    private static Sprite MakeSprite(Texture2D tex, Vector2 pivot)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, tex.height);
    }
}
