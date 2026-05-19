using UnityEngine;
using System.Collections.Generic;
// Hero.cs
// This goes on your Paladin GameObject in the scene.
// In the Inspector, you drag in the Paladin.asset
// into the heroData field.

public class Hero : Character
{
    public HeroData HeroData => heroData;
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
    public override float AttackRange => heroData.attackRange;
    public override float AttackRate => heroData.attackRate;

    public override string GetCharacterName() => heroData.heroName;

    // ─────────────────────────────────────────
    // ABILITY HANDLERS
    // One AbilityHandler component is added per
    // ability when the Hero wakes up
    // ─────────────────────────────────────────

    private List<AbilityHandler> abilityHandlers = new List<AbilityHandler>();
    private bool initialized = false;
    public void Init(HeroData data, float savedHealth = 0f)
    {
        heroData = data;
        currentHealth = (savedHealth > 0f) ? savedHealth : MaxHealth;
        initialized = true;

        // Apply sprite tint early so CharacterSelector.Start() picks it up
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.color = heroData.spriteTint;
    }

    // ─────────────────────────────────────────
    // LIFECYCLE
    // ─────────────────────────────────────────


    private void Start()
    {
        // Set starting health from the data asset
        if (!initialized)
        {
            currentHealth = MaxHealth;
        }
        // Create one AbilityHandler component per ability
        InitializeAbilities();

        Debug.Log($"{GetCharacterName()} is ready! " +
                  $"HP: {currentHealth} | ATK: {AttackDamage} | Armor: {GetArmor()}");
        
        // Hero registers itself with the SquadManager on scene load
        SquadManager.Instance.AddHero(this);
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

    public void ReleaseAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilityHandlers.Count) return;

        abilityHandlers[slotIndex].ReleaseCharge();
    }

}