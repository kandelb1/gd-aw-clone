using Godot;
using System;

public partial class LanderUnit : Node2D
{    
    private Unit unit;
    
    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Lander");
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/LanderMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(6);
        unit.SetMoveDefinition(moveDef);
        unit.SetLoadCapacity(2);
        // unit.SetLoadRules(otherUnit => otherUnit.GetUnitName() is "Infantry" or "MechInfantry");
    }
}
