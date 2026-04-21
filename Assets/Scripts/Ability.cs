using System;
using UnityEngine;

/// <summary>
/// Represents a single ability with a cooldown.
/// Designed to be subclassed or configured for specific ability behaviors.
/// </summary>
[Serializable]
public class Ability
{
    public string abilityName;
    public float cooldownDuration; // seconds
    public string keyBind;        // for display purposes (e.g. "Q", "W", "E", "R")

    private float cooldownTimer = 0f;

    public bool IsReady => cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

    public Ability(string name, float cooldown, string key)
    {
        abilityName = name;
        cooldownDuration = cooldown;
        keyBind = key;
        cooldownTimer = 0f;
    }

    /// <summary>
    /// Attempts to use the ability. Returns true if it fired, false if on cooldown.
    /// </summary>
    public bool TryUse()
    {
        if (!IsReady)
        {
            Debug.Log($"[{keyBind}] {abilityName} is on cooldown! {CooldownRemaining:F1}s remaining.");
            return false;
        }

        // Fire the ability
        Execute();
        cooldownTimer = cooldownDuration;
        Debug.Log($"[{keyBind}] {abilityName} used! Cooldown: {cooldownDuration}s");
        return true;
    }

    /// <summary>
    /// Called each frame to tick down the cooldown timer.
    /// Logs once when the ability comes off cooldown.
    /// </summary>
    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                Debug.Log($"[{keyBind}] {abilityName} is ready!");
            }
        }
    }

    /// <summary>
    /// Override this in subclasses to implement actual ability behavior.
    /// For now, just logs to console.
    /// </summary>
    protected virtual void Execute()
    {
        // Base implementation — placeholder for testing.
        // Subclass and override this to add real behavior
        // (spawn projectiles, apply buffs, deal damage, etc.)
    }

    /// <summary>
    /// Force-reset the cooldown (e.g. for cooldown reduction effects).
    /// </summary>
    public void ResetCooldown()
    {
        cooldownTimer = 0f;
        Debug.Log($"[{keyBind}] {abilityName} cooldown reset!");
    }
}
