using Godot;
using System;

public partial class BomberUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Bomber");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/AirMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(7);
        unit.SetMoveDefinition(moveDef);
        
        Weapon bombs = (Weapon) GD.Load("res://Scenes/Weapons/Bomber/Bombs.tres");
        unit.SetPrimaryWeapon(bombs);
        unit.SetDirectCombat(true);
    }
}
