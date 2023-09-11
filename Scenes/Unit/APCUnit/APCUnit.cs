using Godot;
using System;

public partial class APCUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("APC");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        // APCs have no weapons
    }
}
