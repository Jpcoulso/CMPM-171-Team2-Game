using UnityEngine;
using System.Collections.Generic;

// Hero.cs — A playable hero unit.
// Attach to a hero GameObject. Drag a HeroData asset (e.g. Paladin.asset)
// into the heroData field in the Inspector to configure stats and abilities.

public class Hero : Character
{
    [SerializeField] private HeroData heroData;

    // --- Stats from HeroData ScriptableObject ---
    public override float MaxHealth    => heroData.maxHealth;
    public override float AttackDamage => heroData.attackDamage;
    public override float MoveSpeed    => heroData.moveSpeed;
    public override float AttackRange  => heroData.attackRange;
    public override float AttackRate   => heroData.attackRate;
    public override float GetArmor()   => heroData.armor;
    public override string GetCharacterName() => heroData.heroName;

    // --- Abilities ---
    private List<AbilityHandler> abilityHandlers = new List<AbilityHandler>();

    // Public accessors for UI (TeamHUD reads these)
    public HeroData HeroDataAsset => heroData;
    public IReadOnlyList<AbilityHandler> AbilityHandlers => abilityHandlers;

    // =============================================
    //  LIFECYCLE
    // =============================================

    private void Start()
    {
        currentHealth = MaxHealth;
        InitializeAbilities();
        SquadManager.Instance.AddHero(this);
    }

    // Creates one AbilityHandler component per ability in the HeroData asset
    private void InitializeAbilities()
    {
        foreach (AbilityData abilityData in heroData.abilities)
        {
            AbilityHandler handler = gameObject.AddComponent<AbilityHandler>();
            handler.Initialize(abilityData, this);
            abilityHandlers.Add(handler);
        }
    }

    // =============================================
    //  OVERRIDES
    // =============================================

    protected override void OnDamageTaken(float amount)
    {
        // TODO: flash health bar, play hurt animation
    }

    protected override void OnDeath()
    {
        // TODO: notify SquadManager, play death animation, disable movement
    }

    // =============================================
    //  PUBLIC — Called by InputManager to fire abilities
    // =============================================

    // Activates an ability by slot index (0=Q, 1=W, 2=E, 3=R)
    public void UseAbility(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= abilityHandlers.Count)
            return;

        abilityHandlers[slotIndex].TryActivate();
    }
}
