using Godot;
using System;

public partial class BattleshipUnit : Node2D
{
    private Unit unit;

    public override void _Ready()
    {
        unit = GetNode<Unit>("Unit");
        // configure things specific to this unit, like its movement and weapons
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/ShipMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(5);
        unit.SetMoveDefinition(moveDef);
        unit.SetUnitName("Battleship");
        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/Battleship/Cannon.tres");
        unit.SetPrimaryWeapon(cannon);
        unit.SetDirectCombat(false, 2, 6);
    }
}