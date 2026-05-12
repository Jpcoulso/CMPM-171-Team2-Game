using UnityEngine;

// WarCryData.cs — Warrior E ability.
// Grants a temporary shield that absorbs damage. White border shows while active.
// In Unity: Right Click -> Create -> RPG/Abilities/War Cry

[CreateAssetMenu(fileName = "WarCry", menuName = "RPG/Abilities/War Cry")]
public class WarCryData : AbilityData
{
    [Header("War Cry Settings")]
    public float shieldAmount = 30f;
    public float shieldDuration = 1f;
    public float ringStartSize = 0.5f;
    public float ringEndSize = 3.5f;
    public float vfxDuration = 0.5f;

    public override void Execute(Character owner)
    {
        Vector3 center = owner.transform.position;

        // Apply shield buff (white border appears on the character)
        owner.ApplyShield(shieldAmount, shieldDuration);

        // Yellow expanding ring VFX
        AbilityVFX.SpawnRing(center, ringStartSize, ringEndSize,
            new Color(1f, 0.9f, 0.2f, 0.7f), vfxDuration);

        // Small flash on the unit
        AbilityVFX.SpawnSquare(center, 1.0f,
            new Color(1f, 1f, 0.5f, 0.5f), 0.2f);
    }

    public override void ApplyPassive(Character owner) { }
}
