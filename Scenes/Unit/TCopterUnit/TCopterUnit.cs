using Godot;
using System;

public partial class TCopterUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("TCopter");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        
        // TCopters have no weapons
    }
}
