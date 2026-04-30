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

    public abstract void Execute(Character owner);
    public abstract void ApplyPassive(Character owner);
}
