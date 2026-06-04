using UnityEngine;
using UnityEngine.InputSystem;

// Medium heal to the ally closest to the cursor. 7 second cooldown.

[CreateAssetMenu(fileName = "GuardianAngel", menuName = "RPG/Abilities/Guardian Angel")]
public class GuardianAngelAbilityData : AbilityData
{
    [Header("Guardian Angel Settings")]
    public float healAmount = 15f;
    public float targetRadius = 3f; // how close cursor needs to be to an ally
    public float visualDuration = 0.4f;

    public override void Execute(Character owner)
    {
        // Find the ally closest to the cursor
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mouseWorld, targetRadius);

        Hero closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Hero ally = hit.GetComponent<Hero>();
            if (ally == null || ally.IsDead) continue;

            float dist = Vector2.Distance(mouseWorld, ally.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = ally;
            }
        }

        if (closest == null)
        {
            Debug.Log(owner.GetCharacterName() + " cast Guardian Angel but no ally was near the cursor.");
            return;
        }

        closest.Heal(healAmount);
        Debug.Log(owner.GetCharacterName() + " heals " + closest.GetCharacterName() + " for " + healAmount + " HP!");
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Guardian_sfx);
        // Yellow flash on the healed ally
        SpawnHealVisual(closest.transform.position);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }

    private void SpawnHealVisual(Vector3 position)
    {
        GameObject visual = new GameObject("GuardianAngel_Visual");
        visual.transform.position = position;

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = AbilitySpriteCache.GetCircle(64);
        sr.color = new Color(1f, 0.85f, 0.2f, 0.5f); // gold
        sr.sortingLayerName = "Foreground";
        sr.sortingOrder = 997;

        visual.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        Object.Destroy(visual, visualDuration);
    }
}
