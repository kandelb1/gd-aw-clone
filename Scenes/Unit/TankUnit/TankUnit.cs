using Godot;
using System;

public partial class TankUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Tank");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/Tank/Cannon.tres");
        unit.SetPrimaryWeapon(cannon);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/Tank/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
