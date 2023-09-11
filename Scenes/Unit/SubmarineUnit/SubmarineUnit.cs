using Godot;
using System;

public partial class SubmarineUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Submarine");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/ShipMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(5);
        unit.SetMoveDefinition(moveDef);

        Weapon torpedoes = (Weapon) GD.Load("res://Scenes/Weapons/Submarine/Torpedoes.tres");
        unit.SetPrimaryWeapon(torpedoes);
        unit.SetDirectCombat(true);
    }
}
