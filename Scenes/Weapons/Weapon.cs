using Godot;
using System;
using Godot.Collections;


[GlobalClass]
public partial class Weapon : Resource
{
    [Export] private string weaponName;
    [Export] private int maxAmmo; // max ammo of 0 means infinite ammo
    private int currentAmmo;

    // I don't think this data belongs to the weapon. It belongs to the unit (direct or indirect combat)
    // private int minWeaponRange;
    // private int maxWeaponRange;
    
    // https://warswiki.org/wiki/Damage/Advance_Wars_2_chart
    [Export] private Dictionary<string, int> damageTable;

    public Weapon()
    {
        currentAmmo = maxAmmo;
    }

    public string GetWeaponName() => weaponName;

    public int GetMaxAmmo() => maxAmmo;

    public int GetCurrentAmmo() => currentAmmo;

    public bool HasAmmo() => currentAmmo > 0;

    public int GetBaseDamageAgainstUnit(Unit unit)
    {
        if (damageTable.ContainsKey(unit.GetUnitName()))
        {
            return damageTable[unit.GetUnitName()];
        }
        
        return -1;
    }
}
