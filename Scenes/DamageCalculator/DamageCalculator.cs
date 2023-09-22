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
        return answer;
    }

    public static int CalculateDamageWithWeapon(Unit attackingUnit, Weapon weapon, Unit defendingUnit)
    {
        int visualHealth = attackingUnit.GetVisualHealth();
        int baseDamage = weapon.GetBaseDamageAgainstUnit(defendingUnit); // assume that the weapon can attacking the defending unit

        float healthModifier = visualHealth / 10f;
        
        int defense = Level.Instance.GetDefense(defendingUnit.GetGridPosition());
        float defenseModifier = 1 - (defense * visualHealth / 100f);
        if (defendingUnit.GetUnitType() is UnitType.Copter or UnitType.Plane)
        {
            defenseModifier = 1f;
        }
        
        int answer = Mathf.FloorToInt(baseDamage * healthModifier * defenseModifier);
        return answer;
    }

    // returns which weapon the attacking unit will be using
    public static Weapon GetWeaponToUse(Unit attackingUnit, Unit defendingUnit)
    {
        // try the primary first, then the secondary
        Weapon primary = attackingUnit.GetPrimaryWeapon();
        Weapon secondary = attackingUnit.GetSecondaryWeapon();
        int primaryDamage = -1;
        int secondaryDamage = -1;
        
        if (primary != null && primary.HasAmmo())
        {
            primaryDamage = primary.GetBaseDamageAgainstUnit(defendingUnit);
            // GD.Print($"{primary.GetWeaponName()} does {primaryDamage} against {defendingUnit.GetName()}");
        }
        if (secondary != null)
        {
            secondaryDamage = secondary.GetBaseDamageAgainstUnit(defendingUnit);
            // GD.Print($"{secondary.GetWeaponName()} does {secondaryDamage} against {defendingUnit.GetName()}");
        }
        if (primaryDamage > secondaryDamage)
        {
            // GD.Print("Using primary");
            return primary;
        }
        if (secondaryDamage > primaryDamage)
        {
            // GD.Print("Using secondary");
            return secondary;
        }
        return null; // both weapon damages were -1, which means they can't shoot at the defendingUnit
        // there's also a possible edge case were primaryDamage == secondaryDamage (and they're not -1), but that will never happen in this game
    }


}
