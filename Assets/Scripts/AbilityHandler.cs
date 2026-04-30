using UnityEngine;
// AbilityHandler.cs
// This class no longer knows ANYTHING about specific abilities.
// It only manages the cooldown and delegates behaviour to the data asset.

public class AbilityHandler : MonoBehaviour
{
    private AbilityData data;
    private Character owner;
    private float cooldownTimer;

    public bool IsOnCooldown   => cooldownTimer > 0f;
    public float CooldownTimer => cooldownTimer;
    public AbilityData Data    => data;

    public void Initialize(AbilityData abilityData, Character owner)
    {
        // Note: owner is now Character, not Hero —
        // this means EnemyAbilities could use this same handler later
        this.data  = abilityData;
        this.owner = owner;

        if (data.isPassive)
        {
            data.ApplyPassive(owner);   // ← delegates to the data asset
            Debug.Log($"Passive '{data.abilityName}' activated on " +
                      $"{owner.GetCharacterName()}");
        }
    }

    private void Update()
    {
        if (IsOnCooldown)
            cooldownTimer -= Time.deltaTime;
    }

    public void TryActivate()
    {
        if (data.isPassive)
        {
            Debug.Log($"'{data.abilityName}' is passive — always active.");
            return;
        }

        if (IsOnCooldown)
        {
            Debug.Log($"'{data.abilityName}' on cooldown — " +
                      $"{cooldownTimer:F1}s left.");
            return;
        }

        // ─────────────────────────────────────────
        // THIS is the key line.
        // AbilityHandler has NO idea what Execute does.
        // It just says "do your thing" and the data asset handles it.
        // ─────────────────────────────────────────
        data.Execute(owner);
        cooldownTimer = data.cooldownDuration;
    }
}