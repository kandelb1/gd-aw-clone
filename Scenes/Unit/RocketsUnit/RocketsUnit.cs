using Godot;
using System;

public partial class RocketsUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Rockets");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TiresMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(5);
        unit.SetMoveDefinition(moveDef);
        
        Weapon rockets = (Weapon) GD.Load("res://Scenes/Weapons/Rockets/Rockets.tres");
        unit.SetPrimaryWeapon(rockets);
        unit.SetDirectCombat(false, 3, 5);
    }
}
