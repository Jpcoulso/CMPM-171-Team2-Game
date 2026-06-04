using UnityEngine;

// Small AOE heal around the caster. Heals all allies within range for a flat amount.

[CreateAssetMenu(fileName = "Holy", menuName = "RPG/Abilities/Holy")]
public class HolyAbilityData : AbilityData
{
    [Header("Holy Settings")]
    public float healAmount = 5f;
    public float healRadius = 2f;
    public float visualDuration = 0.4f;

    public override void Execute(Character owner)
    {
        Vector3 heroPos = owner.transform.position;

        // Find all colliders in the heal radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(heroPos, healRadius);

        int healed = 0;
        foreach (Collider2D hit in hits)
        {
            Hero ally = hit.GetComponent<Hero>();
            if (ally == null || ally.IsDead) continue;

            ally.Heal(healAmount);
            healed++;
        }

        Debug.Log(owner.GetCharacterName() + " casts Holy! Healed " + healed + " allies for " + healAmount + " HP.");
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Holy_sfx);
        // Spawn yellow circle visual centered on caster
        SpawnHealVisual(heroPos);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }

    private void SpawnHealVisual(Vector3 position)
    {
        GameObject visual = new GameObject("Holy_Visual");
        visual.transform.position = position;

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetCircle(64);
        sr.color = new Color(1f, 0.9f, 0.3f, 0.4f); // yellow
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 997;

        float diameter = healRadius * 2f;
        visual.transform.localScale = new Vector3(diameter, diameter, 1f);

        Object.Destroy(visual, visualDuration);
    }
}
