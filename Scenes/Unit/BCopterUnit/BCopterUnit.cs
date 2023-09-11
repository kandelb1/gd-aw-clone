using Godot;
using System;

public partial class BCopterUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("BCopter");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        Weapon missiles = (Weapon) GD.Load("res://Scenes/Weapons/BCopter/AirToSurfaceMissiles.tres");
        unit.SetPrimaryWeapon(missiles);
        Weapon machineGun = (Weapon) GD.Load("res://Scenes/Weapons/BCopter/MachineGun.tres");
        unit.SetSecondaryWeapon(machineGun);
        unit.SetDirectCombat(true);
    }
}
