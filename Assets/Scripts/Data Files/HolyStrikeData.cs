using UnityEngine;
// HolyStrikeData.cs
// In Unity: Right Click → Create → RPG/Abilities/Holy Strike
// This creates the HolyStrike.asset you drag into the Paladin's ability list

[CreateAssetMenu(fileName = "HolyStrike", menuName = "RPG/Abilities/Holy Strike")]
public class HolyStrikeData : AbilityData
{
    [Header("Holy Strike Settings")]
    public float damageMultiplier = 2.5f;

    // AbilityData demanded we implement Execute() — here it is.
    // All the Holy Strike specific logic lives HERE, not in AbilityHandler.
    public override void Execute(Character owner)
    {
        float damage = owner.AttackDamage * damageMultiplier;

        Debug.Log($"{owner.GetCharacterName()} uses Holy Strike for {damage} damage!");

        // Later: find nearest enemy, call enemy.TakeDamage(damage)
        // The handler doesn't need to know any of this
    }

    // Holy Strike is not a passive so this does nothing
    public override void ApplyPassive(Character owner) { }
}
