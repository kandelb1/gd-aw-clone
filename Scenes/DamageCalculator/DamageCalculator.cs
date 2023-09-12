using Godot;
using System;

public partial class DamageCalculator : Node
{

    // https://gamefaqs.gamespot.com/gba/589391-advance-wars-2-black-hole-rising/faqs/33510
    public static int CalculateDamage(Unit attackingUnit, Unit defendingUnit)
    {
        Weapon weaponInUse = GetWeaponToUse(attackingUnit, defendingUnit);
        if (weaponInUse == null) // this shouldn't happen, but doesn't hurt to check
        {
            GD.PrintErr($"Error in CalculateDamage(): {attackingUnit.GetUnitName()} cannot shoot at {defendingUnit.GetUnitName()}");
            return 0;
        }

        int visualHealth = GetVisualHealth(attackingUnit);
        int baseDamage = weaponInUse.GetBaseDamageAgainstUnit(defendingUnit);
        
        float healthModifier = visualHealth / 10f;
        
        int defense = Level.Instance.GetDefense(defendingUnit.GetGridPosition());
        float defenseModifier = 1 - (defense * visualHealth / 100f);
        
        int answer = Mathf.FloorToInt(baseDamage * healthModifier * defenseModifier);
        GD.Print($"{attackingUnit.GetUnitName()} with {attackingUnit.GetHealth()} health attacking {defendingUnit.GetUnitName()} on a {defense}-star tile ---- {answer} damage.");
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

    public static int GetVisualHealth(Unit unit)
    {
        switch (unit.GetHealth())
        {
            case <= 100 and >= 91:
                return 10;
            case <= 90 and >= 81:
                return 9;
            case <= 80 and >= 71:
                return 8;
            case <= 70 and >= 61:
                return 7;
            case <= 60 and >= 51:
                return 6;
            case <= 50 and >= 41:
                return 5;
            case <= 40 and >= 31:
                return 4;
            case <= 30 and >= 21:
                return 3;
            case <= 20 and >= 11:
                return 2;
            case <= 10 and >= 1:
                return 1;
            default:
                return 0;
        }
    }
}
