using Godot;
using System;
using Godot.Collections;


[GlobalClass]
public partial class Weapon : Resource
{
    [Export] private string weaponName;
    [Export] private int maxAmmo; // max ammo of 0 means infinite ammo
    private int currentAmmo;

    // https://warswiki.org/wiki/Damage/Advance_Wars_2_chart
    [Export] private Dictionary<string, int> damageTable;

    public string GetWeaponName() => weaponName;

    public int GetMaxAmmo() => maxAmmo;

    public int GetCurrentAmmo() => currentAmmo;

    public void SetCurrentAmmo(int ammo) => currentAmmo = ammo;

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
