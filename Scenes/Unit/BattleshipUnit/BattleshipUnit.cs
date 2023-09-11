using Godot;
using System;

public partial class BattleshipUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Battleship");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/ShipMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(5);
        unit.SetMoveDefinition(moveDef);

        Weapon cannon = (Weapon) GD.Load("res://Scenes/Weapons/Battleship/Cannon.tres");
        unit.SetPrimaryWeapon(cannon);
        unit.SetDirectCombat(false, 2, 6);
    }
}