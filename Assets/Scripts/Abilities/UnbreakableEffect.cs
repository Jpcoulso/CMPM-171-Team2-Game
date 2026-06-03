using UnityEngine;

// UnbreakableEffect.cs
// Added to each ally when Unbreakable is cast.
// Makes the character immune to all damage and shows a white ring border.

public class UnbreakableEffect : MonoBehaviour
{
    private Character character;
    private GameObject ringVisual;
    private float duration;
    private float timer;

    public void Initialize(Character target, float effectDuration)
    {
        character = target;
        duration = effectDuration;
        timer = 0f;

        // Make this character immune
        character.isInvulnerable = true;
        character.Heal(character.MaxHealth / 4);

        // Create a ring visual around the character
        ringVisual = new GameObject("UnbreakableRing");
        ringVisual.transform.SetParent(character.transform);
        ringVisual.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = ringVisual.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetRing(64, 0.85f);
        sr.color = new Color(0.9f, 0.9f, 0.95f, 0.6f); // light gray
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 998;

        // Scale the ring to be slightly larger than the character
        ringVisual.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration || character == null || character.IsDead)
        {
            Expire();
            return;
        }

        // Pulse the ring alpha slightly
        if (ringVisual != null)
        {
            SpriteRenderer sr = ringVisual.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float alpha = 0.5f + 0.2f * Mathf.Sin(timer * 6f);
                Color c = sr.color;
                sr.color = new Color(c.r, c.g, c.b, alpha);
            }
        }
    }

    private void Expire()
    {
        if (character != null)
            character.isInvulnerable = false;

        if (ringVisual != null)
            Destroy(ringVisual);

        Destroy(this); // remove just the component
    }

    private void OnDestroy()
    {
        if (character != null)
            character.isInvulnerable = false;

        if (ringVisual != null)
            Destroy(ringVisual);
    }
}
