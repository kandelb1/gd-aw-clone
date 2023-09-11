using Godot;
using System;

public partial class FighterUnit : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        // configure things specific to this unit, like its movement and weapons
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(9);
        unit.SetMoveDefinition(moveDef);
        unit.SetUnitName("Fighter");
        Weapon missiles = (Weapon) GD.Load("res://Scenes/Weapons/Fighter/AirToAirMissiles.tres");
        unit.SetPrimaryWeapon(missiles);
        unit.SetDirectCombat(true);
    }
}
