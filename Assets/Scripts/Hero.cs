using UnityEngine;
using System.Collections.Generic;
// Hero.cs
// This goes on your Paladin GameObject in the scene.
// In the Inspector, you drag in the Paladin.asset
// into the heroData field.

public class Hero : Character
{
    // ─────────────────────────────────────────
    // THE DATA CONNECTION
    // This is the link between the ScriptableObject
    // asset and the behaviour in this script.
    // ─────────────────────────────────────────

    [SerializeField] private HeroData heroData;

    // ─────────────────────────────────────────
    // FILLING IN THE ABSTRACT PROPERTIES
    // Character.cs demanded these exist.
    // Now we answer: "where do the numbers come from?"
    // Answer: from heroData (the ScriptableObject)
    // ─────────────────────────────────────────

    public override float MaxHealth    => heroData.maxHealth;
    public override float AttackDamage => heroData.attackDamage;
    public override float MoveSpeed    => heroData.moveSpeed;
    public override float GetArmor()   => heroData.armor;
    public override bool IsRanged      => heroData.isRanged;
    public override float AttackRange => heroData.attackRange;

    public override string GetCharacterName() => heroData.heroName;

    // ─────────────────────────────────────────
    // ABILITY HANDLERS
    // One AbilityHandler component is added per
    // ability when the Hero wakes up
    // ─────────────────────────────────────────

    private List<AbilityHandler> abilityHandlers = new List<AbilityHandler>();

    // ─────────────────────────────────────────
    // LIFECYCLE
    // ─────────────────────────────────────────

    private void Start()
    {
        // Set starting health from the data asset
        currentHealth = MaxHealth;

        // Create one AbilityHandler component per ability
        InitializeAbilities();

        Debug.Log($"{GetCharacterName()} is ready! " +
                  $"HP: {currentHealth} | ATK: {AttackDamage} | Armor: {GetArmor()}");
    }

    private void InitializeAbilities()
    {
        foreach (AbilityData abilityData in heroData.abilities)
        {
            // We add the component in code, not the Inspector,
            // because we don't know ahead of time how many
            // abilities this hero will have
            AbilityHandler handler = gameObject.AddComponent<AbilityHandler>();
            handler.Initialize(abilityData, this);
            abilityHandlers.Add(handler);
            Debug.Log("ability handler succesfully initialized!");
        }
    }


    // ─────────────────────────────────────────
    // MOVEMENT
    // ─────────────────────────────────────────
    protected override void MoveTowards(Vector3 position)
    {
        
    }
    protected override void MoveToDestination(){}
    protected override void FaceTarget(Vector3 position){}
    protected override bool HasReachedDestination(){ return false;}


    // ─────────────────────────────────────────
    // HERO-SPECIFIC OVERRIDES
    // ─────────────────────────────────────────

    protected override void OnDamageTaken(float amount)
    {
        // Later: flash the health bar, play a hurt animation
    }

    protected override void OnDeath()
    {
        // Later: notify SquadManager, play death animation,
        // disable movement, etc.
        Debug.Log($"{GetCharacterName()} has fallen in battle!");
    }

    protected override void AggroEnemy(Character targetEnemy)
    {
        if (IsRanged)
        {
            Debug.Log($"{GetCharacterName()} shoots at {Target}");
        }
    }

    // ─────────────────────────────────────────
    // PUBLIC INTERFACE
    // Other systems (like a UI button) call this
    // to trigger an ability by its slot number
    // ─────────────────────────────────────────

    public void UseAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilityHandlers.Count)
        {
            Debug.LogWarning("Ability slot index out of range.");
            return;
        }

        abilityHandlers[slotIndex].TryActivate();
    }

}