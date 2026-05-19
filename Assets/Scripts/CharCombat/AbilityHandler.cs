using UnityEngine;
// AbilityHandler.cs
// Manages cooldown and delegates behaviour to the data asset.
// Now also supports hold-to-charge abilities.

public class AbilityHandler : MonoBehaviour
{
    private AbilityData data;
    private Character owner;
    private float cooldownTimer;
    private float chargeTimer;
    private bool charging;

    public bool IsOnCooldown   => cooldownTimer > 0f;
    public float CooldownTimer => cooldownTimer;
    public AbilityData Data    => data;
    public bool IsCharging     => charging;

    public void Initialize(AbilityData abilityData, Character owner)
    {
        this.data  = abilityData;
        this.owner = owner;

        if (data.isPassive)
        {
            data.ApplyPassive(owner);
            Debug.Log("Passive '" + data.abilityName + "' activated on " + owner.GetCharacterName());
        }
    }

    private void Update()
    {
        if (IsOnCooldown)
            cooldownTimer -= Time.deltaTime;

        if (charging)
            chargeTimer += Time.deltaTime;
    }

    public void TryActivate()
    {
        if (data.isPassive)
        {
            Debug.Log("'" + data.abilityName + "' is passive — always active.");
            return;
        }

        if (IsOnCooldown)
        {
            Debug.Log("'" + data.abilityName + "' on cooldown — " + cooldownTimer.ToString("F1") + "s left.");
            return;
        }

        // Charge abilities: start charging on key press, fire on key release
        if (data.isChargeAbility)
        {
            StartCharge();
            return;
        }

        // Normal instant abilities
        data.Execute(owner);
        cooldownTimer = data.cooldownDuration;
    }

    private void StartCharge()
    {
        if (charging) return;
        charging = true;
        chargeTimer = 0f;
        data.OnChargeStart(owner);
        Debug.Log("Charging '" + data.abilityName + "'...");
    }

    public void ReleaseCharge()
    {
        if (!charging) return;
        charging = false;
        Debug.Log("Released '" + data.abilityName + "' after " + chargeTimer.ToString("F2") + "s charge.");
        data.OnChargeRelease(owner, chargeTimer);
        cooldownTimer = data.cooldownDuration;
    }
}
