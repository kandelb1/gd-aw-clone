using Godot;
using System;

public partial class MechInfantry : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        // configure things specific to this unit, like its movement and weapons
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/MechInfantryMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(2);
        unit.SetMoveDefinition(moveDef);
        unit.SetUnitName("MechInfantry");

        Weapon bazooka = (Weapon) GD.Load("res://Scenes/Weapons/MechInfantry/Bazooka.tres");
        unit.SetPrimaryWeapon(bazooka);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/MechInfantry/MachineGun.tres"); 
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
