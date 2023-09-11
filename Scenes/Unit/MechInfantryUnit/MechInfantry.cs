using Godot;
using System;

public partial class MechInfantry : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("MechInfantry");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/MechInfantryMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(2);
        unit.SetMoveDefinition(moveDef);

        Weapon bazooka = (Weapon) GD.Load("res://Scenes/Weapons/MechInfantry/Bazooka.tres");
        unit.SetPrimaryWeapon(bazooka);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/Infantry/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
