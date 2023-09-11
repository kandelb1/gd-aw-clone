using Godot;
using System;

public partial class NeoTankUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("NeoTank");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/NeoTank/NeoCannon.tres");
        unit.SetPrimaryWeapon(cannon);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/NeoTank/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
