using UnityEngine;

// FireballProjectile.cs
// A fireball that flies from the hero to the target position, then explodes in an AOE.

public class FireballProjectile : MonoBehaviour
{
    private Vector3 targetPos;
    private float speed;
    private float damage;
    private float explosionRadius;
    private bool exploded;

    public void Initialize(Vector3 target, float projSpeed, float projDamage, float radius)
    {
        targetPos = target;
        speed = projSpeed;
        damage = projDamage;
        explosionRadius = radius;
        exploded = false;
    }

    private void Update()
    {
        if (exploded) return;

        // Move toward target
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Check if arrived
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        exploded = true;

        Debug.Log("Fireball explodes for " + damage + " damage! Radius: " + explosionRadius.ToString("F1"));

        // AOE damage to all enemies in the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        int enemiesHit = 0;
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
                enemiesHit++;
                Debug.Log("Fireball hit " + enemy.GetCharacterName() + " for " + damage + " damage!");
            }
        }

        if (enemiesHit == 0)
        {
            Debug.Log("Fireball hit no enemies.");
        }

        // Spawn explosion visual
        SpawnExplosionVisual();

        // Destroy the projectile (the fireball visual gets destroyed with it)
        Destroy(gameObject);
    }

    private void SpawnExplosionVisual()
    {
        GameObject explosion = new GameObject("FireballExplosion");
        explosion.transform.position = transform.position;

        SpriteRenderer sr = explosion.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetCircle(64);
        sr.color = new Color(1f, 0.4f, 0f, 0.55f); //orange
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 999;

        float diameter = explosionRadius * 2f;
        explosion.transform.localScale = new Vector3(diameter, diameter, 1f);

        Destroy(explosion, 0.35f);
    }

}
