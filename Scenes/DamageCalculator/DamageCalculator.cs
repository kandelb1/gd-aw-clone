using Godot;
using System;
using static UnitDefinition;

public partial class DamageCalculator : Node
{

    // https://gamefaqs.gamespot.com/gba/589391-advance-wars-2-black-hole-rising/faqs/33510
    public static int CalculateDamage(Unit attackingUnit, Unit defendingUnit)
    {
        Weapon weaponInUse = GetWeaponToUse(attackingUnit, defendingUnit);
        if (weaponInUse == null) // this shouldn't happen, but doesn't hurt to check
        {
            GD.PrintErr($"Error in CalculateDamage(): {attackingUnit.GetName()} cannot shoot at {defendingUnit.GetName()}");
            return 0;
        }

        int visualHealth = attackingUnit.GetVisualHealth();
        int baseDamage = weaponInUse.GetBaseDamageAgainstUnit(defendingUnit);
        
        float healthModifier = visualHealth / 10f;
        
        int defense = Level.Instance.GetDefense(defendingUnit.GetGridPosition());
        float defenseModifier = 1 - (defense * visualHealth / 100f);
        if (defendingUnit.GetUnitType() is UnitType.Copter or UnitType.Plane)
        {
            defenseModifier = 1f;
        }
        
        int answer = Mathf.FloorToInt(baseDamage * healthModifier * defenseModifier);
        GD.Print($"{attackingUnit.GetName()} with {attackingUnit.GetHealth()} health attacking {defendingUnit.GetName()} on a {defense}-star tile ---- {answer} damage.");
        return answer;
    }

    // returns which weapon the attacking unit will be using
    private static Weapon GetWeaponToUse(Unit attackingUnit, Unit defendingUnit)
    {
        // try the primary first, then the secondary
        Weapon primary = attackingUnit.GetPrimaryWeapon();
        Weapon secondary = attackingUnit.GetSecondaryWeapon();
        // TODO: choose the weapon that does more damage to the defendingUnit
        if (primary != null && primary.HasAmmo() && primary.GetBaseDamageAgainstUnit(defendingUnit) != -1)
        {
            return primary;
        }
        if (secondary != null && secondary.GetBaseDamageAgainstUnit(defendingUnit) != -1)
        {
            return secondary;
        }

        return null;
    }


}
