using Godot;
using System;

public partial class APCUnit : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("APC");
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TreadsMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        unit.SetLoadCapacity(2);
        unit.SetLoadRules(otherUnit => otherUnit.GetUnitName() is "Infantry" or "MechInfantry");
    }
}
