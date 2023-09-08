using Godot;
using System;

public partial class TCopterUnit : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("TCopter");
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        unit.SetLoadCapacity(1);
        unit.SetLoadRules(otherUnit => otherUnit.GetUnitName() is "Infantry" or "MechInfantry");
    }
}
