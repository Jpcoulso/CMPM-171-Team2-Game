/// <summary>
/// Creates the correct Ability subclass based on class name and slot.
/// Falls back to the base Ability for any unimplemented combo.
/// Slot: 0=Q, 1=W, 2=E, 3=R
/// </summary>
public static class AbilityFactory
{
    public static Ability Create(string className, int slot, string name, float cooldown, string key)
    {
        if (className == "Warrior")
        {
            switch (slot)
            {
                case 0: return new ShieldBashAbility(name, cooldown, key);
                case 1: return new CleaveAbility(name, cooldown, key);
                case 2: return new WarCryAbility(name, cooldown, key);
                case 3: return new BerserkerRageAbility(name, cooldown, key);
            }
        }

        return new Ability(name, cooldown, key);
    }
}
