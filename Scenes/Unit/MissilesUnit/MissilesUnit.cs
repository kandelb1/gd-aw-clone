using Godot;
using System;

public partial class MissilesUnit : Node2D
{
    [Export] private bool enemy;
    [Export] private Unit.Team team;
    
    private Unit unit;

    public override void _EnterTree()
    {
        unit = GetNode<Unit>("Unit");
        unit.SetUnitName("Missiles");
        unit.SetEnemy(enemy);
        unit.SetTeam(team);
        
        MoveDefinition moveDef = (MoveDefinition) GD.Load("res://Scenes/Unit/MoveDefinitions/TiresMoveDefinition.tres").Duplicate();
        moveDef.SetMoveDistance(4);
        unit.SetMoveDefinition(moveDef);
        
        Weapon missiles = (Weapon) GD.Load("res://Scenes/Weapons/Missiles/Missiles.tres");
        unit.SetPrimaryWeapon(missiles);
        unit.SetDirectCombat(false, 3, 5);
    }
}
