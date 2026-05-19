using UnityEngine;
using System.Collections.Generic;

// MagicCircleEffect.cs
// A persistent zone on the map. Enemies inside take 2x damage.
// Lasts for a set duration, then cleans up and resets all affected enemies.

public class MagicCircleEffect : MonoBehaviour
{
    private float radius;
    private float damageMultiplier;
    private float duration;
    private float timer;
    private float scanTimer;
    private float scanInterval = 1f; // check once per second
    private SpriteRenderer cachedSR;
    private HashSet<Character> empoweredTargets = new HashSet<Character>();

    public void Initialize(float circleRadius, float dmgMultiplier, float circleDuration)
    {
        radius = circleRadius;
        damageMultiplier = dmgMultiplier;
        duration = circleDuration;
        timer = 0f;
        scanTimer = 0f;
        cachedSR = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            Expire();
            return;
        }

        // Only scan for enemies periodically, not every frame
        scanTimer -= Time.deltaTime;
        if (scanTimer <= 0f)
        {
            scanTimer = scanInterval;
            UpdateEmpowerment();
        }

        // Pulse the visual slightly to show it's active
        if (cachedSR != null)
        {
            float alpha = 0.45f + 0.15f * Mathf.Sin(timer * 3f);
            Color c = cachedSR.color;
            cachedSR.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    private void UpdateEmpowerment()
    {
        // Find all colliders inside the circle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        // Track who is currently inside
        HashSet<Character> currentlyInside = new HashSet<Character>();

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                currentlyInside.Add(enemy);

                // Apply multiplier if not already empowered
                if (!empoweredTargets.Contains(enemy))
                {
                    enemy.incomingDamageMultiplier = damageMultiplier;
                    empoweredTargets.Add(enemy);
                }
            }
        }

        // Reset multiplier for enemies that left the circle
        List<Character> toRemove = new List<Character>();
        foreach (Character target in empoweredTargets)
        {
            if (target == null || !currentlyInside.Contains(target))
            {
                if (target != null)
                    target.incomingDamageMultiplier = 1f;
                toRemove.Add(target);
            }
        }

        foreach (Character target in toRemove)
        {
            empoweredTargets.Remove(target);
        }
    }

    private void Expire()
    {
        // Reset all empowered enemies back to normal
        foreach (Character target in empoweredTargets)
        {
            if (target != null)
                target.incomingDamageMultiplier = 1f;
        }
        empoweredTargets.Clear();

        Debug.Log("Magic Circle expired.");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Safety: reset anyone still empowered
        foreach (Character target in empoweredTargets)
        {
            if (target != null)
                target.incomingDamageMultiplier = 1f;
        }
    }
}
