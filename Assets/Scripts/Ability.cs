using System;
using UnityEngine;

// Ability.cs — LEGACY: Old ability base class.
// Replaced by AbilityData (ScriptableObject) + AbilityHandler system.
// Kept for reference. Safe to delete once fully migrated.

[Serializable]
public class Ability
{
    public string abilityName;
    public float cooldownDuration;
    public string keyBind;

    private float cooldownTimer = 0f;

    public bool IsReady => cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

    [NonSerialized] public GameObject owner;

    public Ability(string name, float cooldown, string key)
    {
        abilityName = name;
        cooldownDuration = cooldown;
        keyBind = key;
    }

    public bool TryUse()
    {
        if (!IsReady) return false;
        Execute();
        cooldownTimer = cooldownDuration;
        return true;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0f)
                cooldownTimer = 0f;
        }
    }

    // Override in subclasses to implement actual ability behavior.
    protected virtual void Execute() { }

    public void ResetCooldown()
    {
        cooldownTimer = 0f;
    }
}
