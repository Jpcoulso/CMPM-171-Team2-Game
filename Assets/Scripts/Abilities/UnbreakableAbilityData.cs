using UnityEngine;
using System.Collections.Generic;

// Every ally becomes immune to all damage for 1 second. 20 second cooldown.

[CreateAssetMenu(fileName = "Unbreakable", menuName = "RPG/Abilities/Unbreakable")]
public class UnbreakableAbilityData : AbilityData
{
    [Header("Unbreakable Settings")]
    public float immunityDuration = 1f;

    public override void Execute(Character owner)
    {
      
        List<Hero> squad = new List<Hero>();
        if (SquadManager.Instance != null)
            squad.AddRange(SquadManager.Instance.GetSquad());
        else
            squad.AddRange(Object.FindObjectsByType<Hero>(FindObjectsSortMode.None));

        int count = 0;
        foreach (Hero hero in squad)
        {
            if (hero == null || hero.IsDead) continue;

            if (hero.isInvulnerable) continue;

            UnbreakableEffect effect = hero.gameObject.AddComponent<UnbreakableEffect>();
            effect.Initialize(hero, immunityDuration);
            count++;
        }

        Debug.Log(owner.GetCharacterName() + " casts Unbreakable! " + count + " allies immune for " + immunityDuration + "s.");
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Unbreakable_sfx);
    }

    public override void ApplyPassive(Character owner)
    {
        // Not passive
    }
}
