using Godot;
using System;

public partial class InfantryUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;

    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Infantry");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);

        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/InfantryMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(3);
        unit.SetMoveDefinition(moveDef);

        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/Infantry/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
