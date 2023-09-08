using Godot;
using System;

public partial class ReconUnit : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        // configure things specific to this unit, like its movement and weapons
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TiresMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(8);
        unit.SetMoveDefinition(moveDef);
        unit.SetUnitName("Recon");
    }
}
