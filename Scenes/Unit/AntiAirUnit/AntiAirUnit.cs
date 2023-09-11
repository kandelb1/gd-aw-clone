using Godot;
using System;

public partial class AntiAirUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("AntiAir");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/AntiAir/VulcanCannon.tres");
        unit.SetPrimaryWeapon(cannon);
        unit.SetDirectCombat(true);
    }
}
