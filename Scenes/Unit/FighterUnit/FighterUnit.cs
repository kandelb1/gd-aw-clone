using Godot;
using System;

public partial class FighterUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Fighter");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(9);
        unit.SetMoveDefinition(moveDef);
        
        Weapon missiles = (Weapon) GD.Load("res://Scenes/Weapons/Fighter/AirToAirMissiles.tres");
        unit.SetPrimaryWeapon(missiles);
        unit.SetDirectCombat(true);
    }
}
