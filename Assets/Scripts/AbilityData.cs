using UnityEngine;
// template for special ability scriptable objects
public abstract class AbilityData : ScriptableObject
{
    [Header("Identity")]
    public string abilityName;
    public Sprite icon;

    [Header("Behaviour")]
    public bool isPassive;
    public float cooldownDuration;

    [TextArea]
    public string description;

    // Override this to return true for hold-to-charge abilities (e.g. Fireball)
    public virtual bool isChargeAbility => false;

    public abstract void Execute(Character owner);
    public abstract void ApplyPassive(Character owner);

    // Charge ability hooks — override these in charge ability subclasses
    public virtual void OnChargeStart(Character owner) { }
    public virtual void OnChargeRelease(Character owner, float chargeTime) { }
}
