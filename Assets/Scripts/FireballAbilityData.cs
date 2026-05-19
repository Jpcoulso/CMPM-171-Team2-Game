using UnityEngine;
using UnityEngine.InputSystem;

// Hold Q/W to charge, release to fire. Longer charge = bigger AOE + more damage.
// Max charge time is 5 seconds. 8 second cooldown.
// In Unity: Right Click in Assets -> Create -> RPG -> Abilities -> Fireball

[CreateAssetMenu(fileName = "Fireball", menuName = "RPG/Abilities/Fireball")]
public class FireballAbilityData : AbilityData
{
    [Header("Fireball Settings")]
    public float minDamage = 30f;
    public float maxDamage = 150f;  
    public float minRadius = 0.5f; 
    public float maxRadius = 2.5f;
    public float maxChargeTime = 5f;
    public float projectileSpeed = 12f;
    public float projectileSize = 0.4f;

    // This is a charge ability
    public override bool isChargeAbility => true;

    
    public override void Execute(Character owner) { }
    public override void ApplyPassive(Character owner) { }

    public override void OnChargeStart(Character owner)
    {
        FireballChargeEffect effect = owner.gameObject.AddComponent<FireballChargeEffect>();
        effect.Initialize(maxChargeTime, minRadius, maxRadius);
    }

    public override void OnChargeRelease(Character owner, float chargeTime)
    {
        float clampedCharge = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        float t = Mathf.Clamp01(clampedCharge / maxChargeTime);

        // Scale damage and radius based on charge
        float damage = Mathf.Lerp(minDamage, maxDamage, t);
        float radius = Mathf.Lerp(minRadius, maxRadius, t);

        FireballChargeEffect effect = owner.GetComponent<FireballChargeEffect>();
        Vector3 targetPos;
        if (effect != null)
        {
            targetPos = effect.GetTargetPosition();
            effect.Cleanup();
        }
        else
        {
            // Fallback: fire toward cursor
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            targetPos = Camera.main.ScreenToWorldPoint(mouseScreen);
            targetPos.z = 0f;
        }

        Debug.Log("Fireball launched! Charge: " + clampedCharge.ToString("F1") + "s | Damage: " + damage.ToString("F0") + " | Radius: " + radius.ToString("F1"));

        // Spawn the fireball projectile from the hero
        SpawnFireball(owner.transform.position, targetPos, damage, radius, t);
    }

    private void SpawnFireball(Vector3 origin, Vector3 target, float damage, float radius, float chargePercent)
    {
        GameObject fireball = new GameObject("Fireball");
        fireball.transform.position = origin;

        SpriteRenderer sr = fireball.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetCircle(32);
        sr.color = new Color(1f, 0.35f, 0f, 0.9f); // bright orange
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 998;

        float visualSize = Mathf.Lerp(projectileSize, projectileSize * 3f, chargePercent);
        fireball.transform.localScale = new Vector3(visualSize, visualSize, 1f);

        FireballProjectile proj = fireball.AddComponent<FireballProjectile>();
        proj.Initialize(target, projectileSpeed, damage, radius);
    }

}
