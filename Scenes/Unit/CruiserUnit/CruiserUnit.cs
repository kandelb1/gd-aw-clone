using Godot;
using System;

public partial class CruiserUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Cruiser");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/ShipMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        Weapon missiles = (Weapon) GD.Load("res://Scenes/Weapons/Cruiser/Missiles.tres");
        unit.SetPrimaryWeapon(missiles);
        Weapon antiAirGun = (Weapon) GD.Load("res://Scenes/Weapons/Cruiser/AAGun.tres");
        unit.SetSecondaryWeapon(antiAirGun);
        unit.SetDirectCombat(true);
    }
}
