using Godot;
using System;

public partial class MTankUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("MTank");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(5);
        unit.SetMoveDefinition(moveDef);
        
        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/MTank/Cannon.tres");
        unit.SetPrimaryWeapon(cannon);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/MTank/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
