using UnityEngine;
// template for special ability scriptable objects
[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptable Objects/AbilityData")]
public class AbilityData : ScriptableObject
{
    [Header("Identity")]
    public string abilityName;
    public Sprite icon;

    [Header("Behaviour")]
    public bool isPassive;

    [Header("Active Only")]
    public float cooldownDuration;
    public float damageMultiplier;
    public float healAmount;

    [TextArea]
    public string description;
}
